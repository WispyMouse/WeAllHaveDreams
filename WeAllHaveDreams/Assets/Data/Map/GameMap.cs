using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMap
{
    Dictionary<MapCoordinates, IEnumerable<MapCoordinates>> Neighbors { get; set; }
    Dictionary<MapCoordinates, GameplayTile> GameplayTiles { get; set; }

    public Realm LoadedRealm;

    public GameplayTile GetGameplayTile(MapCoordinates position)
    {
        GameplayTile foundTile;

        if (GameplayTiles.TryGetValue(position, out foundTile))
        {
            return foundTile;
        }

        return null;
    }

    public static GameMap LoadFromRealm(Realm realmData)
    {
        try
        {
            var gameplayTiles = new Dictionary<MapCoordinates, GameplayTile>();

            GameMap newMap = new GameMap();
            newMap.LoadedRealm = realmData;

            realmData.Hydrate();

            foreach (RealmCoordinate coordinate in realmData.RealmCoordinates)
            {
                gameplayTiles.Add(coordinate.Position, TileLibrary.GetTile(coordinate.Tile));
            }

            newMap.Neighbors = GenerateNeighbors(realmData);
            newMap.GameplayTiles = gameplayTiles;
            return newMap;
        }
        catch (Exception e)
        {
            DebugTextLog.AddTextToLog(e.Message, DebugTextLogChannel.RuntimeError);
            throw e;
        }
    }

    public static GameMap InitializeMapFromTilemap(Tilemap tileMap)
    {
        GameMap newMap = new GameMap();

        var neighbors = new Dictionary<MapCoordinates, IEnumerable<MapCoordinates>>();
        var gameplayTiles = new Dictionary<MapCoordinates, GameplayTile>();

        var traveled = new HashSet<MapCoordinates>() { MapCoordinates.Zero };
        var frontier = new HashSet<MapCoordinates>() { MapCoordinates.Zero };
        gameplayTiles.Add(MapCoordinates.Zero, tileMap.GetTile<GameplayTile>(MapCoordinates.Zero));

        while (frontier.Any())
        {
            MapCoordinates thisTile = frontier.First();
            frontier.Remove(thisTile);

            HashSet<MapCoordinates> actualNeighbors = new HashSet<MapCoordinates>();

            foreach (MapCoordinates curPotentialNeighbor in GetPotentialNeighbors(thisTile))
            {
                // If this tile isn't in the map, continue
                if (!tileMap.HasTile(curPotentialNeighbor))
                {
                    continue;
                }

                actualNeighbors.Add(curPotentialNeighbor);

                // If we've already seen this tile, don't add it to the frontier
                if (traveled.Contains(curPotentialNeighbor))
                {
                    continue;
                }

                frontier.Add(curPotentialNeighbor);
                traveled.Add(curPotentialNeighbor);
                gameplayTiles.Add(curPotentialNeighbor, tileMap.GetTile<GameplayTile>(curPotentialNeighbor));
            }

            neighbors.Add(thisTile, actualNeighbors);
        }

        newMap.Neighbors = neighbors;
        newMap.GameplayTiles = gameplayTiles;

        return newMap;
    }

    public static GameMap LoadEmptyRealm()
    {
        GameMap newMap = new GameMap();
        newMap.LoadedRealm = Realm.GetEmptyRealm();
        newMap.GameplayTiles = new Dictionary<MapCoordinates, GameplayTile>();
        newMap.Neighbors = new Dictionary<MapCoordinates, IEnumerable<MapCoordinates>>();
        return newMap;
    }

    public static MapCoordinates[] GetPotentialNeighbors(MapCoordinates center)
    {
        return new MapCoordinates[]
        {
            center + MapCoordinates.Right,
            center + MapCoordinates.Up,
            center + MapCoordinates.Left,
            center + MapCoordinates.Down
        };
    }

    public IEnumerable<MapCoordinates> PotentialMoves(MapMob movingMob, WorldContext worldContext)
    {
        MapCoordinates startingTile = movingMob.Position;

        var frontier = new Dictionary<MapCoordinates, int>();
        var possibleVisits = new Dictionary<MapCoordinates, int>();

        possibleVisits.Add(startingTile, 0);
        frontier.Add(startingTile, 0);

        while (frontier.Any())
        {
            var thisTile = frontier.First();
            frontier.Remove(thisTile.Key);

            // If we've already expended all our movement, stop moving
            if (thisTile.Value >= movingMob.MoveRange)
            {
                continue;
            }

            foreach (MapCoordinates neighbor in GetNeighbors(thisTile.Key))
            {
                int totalCost = thisTile.Value + 1; // TEMPORARY: This will eventually consider the movement cost of the tile we're moving on to

                if (!CanMoveInTo(movingMob, thisTile.Key, neighbor, worldContext))
                {
                    continue;
                }

                // If we've already considered this tile, and it was more efficient last time, ignore this attempt
                // If we haven't considered this tile, then add it
                if (possibleVisits.ContainsKey(neighbor))
                {
                    if (possibleVisits[neighbor] > totalCost)
                    {
                        possibleVisits[neighbor] = totalCost;

                        // reconsider this as a frontier if it isn't already
                        if (!frontier.ContainsKey(neighbor))
                        {
                            frontier.Add(neighbor, totalCost);
                        }
                    }
                }
                else
                {
                    possibleVisits.Add(neighbor, totalCost);
                    frontier.Add(neighbor, totalCost);
                }
            }
        }

        List<MapCoordinates> possibleMoves = new List<MapCoordinates>();

        foreach (MapCoordinates position in possibleVisits.Keys)
        {
            if (CanStopOn(movingMob, position, worldContext))
            {
                possibleMoves.Add(position);
            }
        }

        return possibleMoves;
    }

    public IEnumerable<MapCoordinates> PotentialAttacks(MapMob attacking, MapCoordinates from)
    {
        MapCoordinates startingTile = from;

        var frontier = new Dictionary<MapCoordinates, int>();
        var possibleAttacks = new Dictionary<MapCoordinates, int>();

        possibleAttacks.Add(startingTile, 0);
        frontier.Add(startingTile, 0);

        while (frontier.Any())
        {
            var thisTile = frontier.First();
            frontier.Remove(thisTile.Key);

            // If we've already expended all our range, stop ranging (lol)
            if (thisTile.Value >= attacking.AttackRange)
            {
                continue;
            }

            foreach (MapCoordinates neighbor in GetNeighbors(thisTile.Key))
            {
                int totalCost = thisTile.Value + 1; // TEMPORARY: This will eventually consider the movement cost of the tile we're moving on to

                // If we've already considered this tile, and it was more efficient last time, ignore this attempt
                // If we haven't considered this tile, then add it
                if (possibleAttacks.ContainsKey(neighbor))
                {
                    if (possibleAttacks[neighbor] > totalCost)
                    {
                        possibleAttacks[neighbor] = totalCost;

                        // reconsider this as a frontier if it isn't already
                        if (!frontier.ContainsKey(neighbor))
                        {
                            frontier.Add(neighbor, totalCost);
                        }
                    }
                }
                else
                {
                    possibleAttacks.Add(neighbor, totalCost);
                    frontier.Add(neighbor, totalCost);
                }
            }
        }

        // Remove the starting position from the list
        possibleAttacks.Remove(from);

        return possibleAttacks.Keys;
    }

    public List<MapCoordinates> Path(MapMob moving, MapCoordinates to, WorldContext worldContext)
    {
        // TEMPORARY: Looks like there's no convenient built in solutions for a Priority Queue, will have to make one
        var frontier = new List<Tuple<MapCoordinates, int>>(); ;
        frontier.Add(new Tuple<MapCoordinates, int>(moving.Position, 0));

        var cameFrom = new Dictionary<MapCoordinates, MapCoordinates>();
        cameFrom.Add(moving.Position, moving.Position);

        var costSoFar = new Dictionary<MapCoordinates, int>();
        costSoFar.Add(moving.Position, 0);

        while (frontier.Any())
        {
            Tuple<MapCoordinates, int> curPositionTuple = frontier.OrderBy(f => f.Item2).First();
            frontier.Remove(curPositionTuple);

            MapCoordinates positionValue = curPositionTuple.Item1;

            if (positionValue == to)
            {
                break;
            }

            foreach (MapCoordinates neighbor in GetNeighbors(positionValue))
            {
                // Can't move here, don't consider it
                if (!CanMoveInTo(moving, positionValue, neighbor, worldContext))
                {
                    continue;
                }

                int newCost = costSoFar[positionValue] + 1; // TEMPORARY: Will eventually consider value of neighbor
                int heuristicDistance = newCost + Mathf.Abs(to.X - neighbor.X) + Mathf.Abs(to.Y - neighbor.Y);

                if (cameFrom.ContainsKey(neighbor))
                {
                    if (newCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = newCost;
                        cameFrom[neighbor] = positionValue;
                        frontier.Add(new Tuple<MapCoordinates, int>(neighbor, heuristicDistance));
                    }
                }
                else
                {
                    costSoFar.Add(neighbor, newCost);
                    cameFrom.Add(neighbor, positionValue);
                    frontier.Add(new Tuple<MapCoordinates, int>(neighbor, heuristicDistance));
                }
            }
        }

        // Weren't able to find the path, return null
        if (!cameFrom.ContainsKey(to))
        {
            return null;
        }

        List<MapCoordinates> path = new List<MapCoordinates>();
        MapCoordinates current = to;

        while (current != moving.Position)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    public IEnumerable<MapCoordinates> CanAttackFrom(MapMob attacking, MapCoordinates target)
    {
        // TEMPORARY: Assume everyone attacks in a perfect area around them, with no diversity in allowed patterns
        // Given this assumption, just find the PotentialAttacks from that position and return that
        // This isn't going to last, but it's handy
        return PotentialAttacks(attacking, target);
    }

    bool CanMoveInTo(MapMob moving, MapCoordinates from, MapCoordinates to, WorldContext worldContext)
    {
        if (!GameplayTiles.ContainsKey(to))
        {
            DebugTextLog.AddTextToLog($"Reporting that {to.ToString()} is off the gameplay map and can't be moved in to");
            return false;
        }

        if (GetGameplayTile(to).CompletelySolid)
        {
            // DebugTextLog.AddTextToLog($"Reporting that {to.x}, {to.y} is completely solid and can't be moved in to");
            return false;
        }

        MapMob mobOnPoint;

        if ((mobOnPoint = worldContext.MobHolder.MobOnPoint(to)) != null && mobOnPoint.PlayerSideIndex != moving.PlayerSideIndex)
        {
            return false;
        }

        return true;
    }

    bool CanStopOn(MapMob moving, MapCoordinates to, WorldContext worldContext)
    {
        if (!GameplayTiles.ContainsKey(to))
        {
            return false;
        }

        if (GetGameplayTile(to).CompletelySolid)
        {
            return false;
        }

        MapMob mobOnPoint;

        if ((mobOnPoint = worldContext.MobHolder.MobOnPoint(to)) != null && mobOnPoint != moving)
        {
            return false;
        }

        return true;
    }

    public IEnumerable<MapCoordinates> GetNeighbors(MapCoordinates point)
    {
        IEnumerable<MapCoordinates> neighbors;

        if (Neighbors.TryGetValue(point, out neighbors))
        {
            return neighbors;
        }

        DebugTextLog.AddTextToLog($"Tried to get neighbors for {point.ToString()}, but the tile wasn't in the {nameof(Neighbors)} dictionary.", DebugTextLogChannel.Verbose);

        return Array.Empty<MapCoordinates>();
    }

    public IEnumerable<MapCoordinates> GetAllTiles()
    {
        return GameplayTiles.Keys;
    }

    public void SetTile(MapCoordinates position, GameplayTile tile)
    {
        if (tile == null)
        {
            RemoveTile(position);
            return;
        }

        if (GameplayTiles.ContainsKey(position))
        {
            GameplayTiles[position] = tile;
        }
        else
        {
            GameplayTiles.Add(position, tile);

            List<MapCoordinates> newNeighbors = new List<MapCoordinates>();

            foreach (MapCoordinates curNeighbor in GetPotentialNeighbors(position))
            {
                if (GameplayTiles.ContainsKey(curNeighbor))
                {
                    Neighbors[curNeighbor] = Neighbors[curNeighbor].Union(new MapCoordinates[] { position });
                    newNeighbors.Add(curNeighbor);
                }
            }

            Neighbors.Add(position, newNeighbors);
        }
    }

    public void RemoveTile(MapCoordinates position)
    {
        GameplayTiles.Remove(position);
        Neighbors.Remove(position);

        foreach (MapCoordinates curNeighbor in GetPotentialNeighbors(position))
        {
            if (GameplayTiles.ContainsKey(curNeighbor))
            {
                Neighbors[curNeighbor] = Neighbors[curNeighbor].Except(new MapCoordinates[] { position });
            }
        }
    }

    static Dictionary<MapCoordinates, IEnumerable<MapCoordinates>> GenerateNeighbors(Realm forRealm)
    {
        Dictionary<MapCoordinates, IEnumerable<MapCoordinates>> neighborsDictionary = new Dictionary<MapCoordinates, IEnumerable<MapCoordinates>>();

        foreach (RealmCoordinate coordinate in forRealm.RealmCoordinates)
        {
            neighborsDictionary.Add(coordinate.Position, new MapCoordinates[] { });
        }

        foreach (RealmCoordinate coordinate in forRealm.RealmCoordinates)
        {
            List<MapCoordinates> actualNeighbors = new List<MapCoordinates>();

            foreach (MapCoordinates neighbor in GetPotentialNeighbors(coordinate.Position))
            {
                if (neighborsDictionary.ContainsKey(neighbor))
                {
                    actualNeighbors.Add(neighbor);
                }
            }

            neighborsDictionary[coordinate.Position] = actualNeighbors;
        }

        return neighborsDictionary;
    }
}
