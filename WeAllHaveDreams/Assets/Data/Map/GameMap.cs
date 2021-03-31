using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMap
{
    Dictionary<Vector3Int, IEnumerable<Vector3Int>> Neighbors { get; set; }
    Dictionary<Vector3Int, GameplayTile> GameplayTiles { get; set; }

    public Realm LoadedRealm;

    public GameplayTile GetGameplayTile(Vector3Int position)
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
            var gameplayTiles = new Dictionary<Vector3Int, GameplayTile>();

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

        var neighbors = new Dictionary<Vector3Int, IEnumerable<Vector3Int>>();
        var gameplayTiles = new Dictionary<Vector3Int, GameplayTile>();

        var traveled = new HashSet<Vector3Int>() { Vector3Int.zero };
        var frontier = new HashSet<Vector3Int>() { Vector3Int.zero };
        gameplayTiles.Add(Vector3Int.zero, tileMap.GetTile<GameplayTile>(Vector3Int.zero));

        while (frontier.Any())
        {
            Vector3Int thisTile = frontier.First();
            frontier.Remove(thisTile);

            HashSet<Vector3Int> actualNeighbors = new HashSet<Vector3Int>();

            foreach (Vector3Int curPotentialNeighbor in GetPotentialNeighbors(thisTile))
            {
                // If this tile isn't in the map, continue
                if (!tileMap.HasTile((Vector3Int)curPotentialNeighbor))
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
                gameplayTiles.Add(curPotentialNeighbor, tileMap.GetTile<GameplayTile>((Vector3Int)curPotentialNeighbor));
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
        newMap.GameplayTiles = new Dictionary<Vector3Int, GameplayTile>();
        newMap.Neighbors = new Dictionary<Vector3Int, IEnumerable<Vector3Int>>();
        return newMap;
    }

    public static Vector3Int[] GetPotentialNeighbors(Vector3Int center)
    {
        return new Vector3Int[]
        {
            center + Vector3Int.right,
            center + Vector3Int.up,
            center + Vector3Int.left,
            center + Vector3Int.down
        };
    }

    public IEnumerable<Vector3Int> PotentialMoves(MapMob movingMob, WorldContext worldContext)
    {
        Vector3Int startingTile = movingMob.Position;

        var frontier = new Dictionary<Vector3Int, int>();
        var possibleVisits = new Dictionary<Vector3Int, int>();

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

            foreach (Vector3Int neighbor in GetNeighbors(thisTile.Key))
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

        List<Vector3Int> possibleMoves = new List<Vector3Int>();

        foreach (Vector3Int position in possibleVisits.Keys)
        {
            if (CanStopOn(movingMob, position, worldContext))
            {
                possibleMoves.Add(position);
            }
        }

        return possibleMoves;
    }

    public IEnumerable<Vector3Int> PotentialAttacks(MapMob attacking, Vector3Int from)
    {
        Vector3Int startingTile = from;

        var frontier = new Dictionary<Vector3Int, int>();
        var possibleAttacks = new Dictionary<Vector3Int, int>();

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

            foreach (Vector3Int neighbor in GetNeighbors(thisTile.Key))
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

    public List<Vector3Int> Path(MapMob moving, Vector3Int to, WorldContext worldContext)
    {
        // TEMPORARY: Looks like there's no convenient built in solutions for a Priority Queue, will have to make one
        var frontier = new List<Tuple<Vector3Int, int>>(); ;
        frontier.Add(new Tuple<Vector3Int, int>(moving.Position, 0));

        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        cameFrom.Add(moving.Position, moving.Position);

        var costSoFar = new Dictionary<Vector3Int, int>();
        costSoFar.Add(moving.Position, 0);

        while (frontier.Any())
        {
            Tuple<Vector3Int, int> curPositionTuple = frontier.OrderBy(f => f.Item2).First();
            frontier.Remove(curPositionTuple);

            Vector3Int positionValue = curPositionTuple.Item1;

            if (positionValue == to)
            {
                break;
            }

            foreach (Vector3Int neighbor in GetNeighbors(positionValue))
            {
                // Can't move here, don't consider it
                if (!CanMoveInTo(moving, positionValue, neighbor, worldContext))
                {
                    continue;
                }

                int newCost = costSoFar[positionValue] + 1; // TEMPORARY: Will eventually consider value of neighbor
                int heuristicDistance = newCost + Mathf.Abs(to.x - neighbor.x) + Mathf.Abs(to.y - neighbor.y);

                if (cameFrom.ContainsKey(neighbor))
                {
                    if (newCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = newCost;
                        cameFrom[neighbor] = positionValue;
                        frontier.Add(new Tuple<Vector3Int, int>(neighbor, heuristicDistance));
                    }
                }
                else
                {
                    costSoFar.Add(neighbor, newCost);
                    cameFrom.Add(neighbor, positionValue);
                    frontier.Add(new Tuple<Vector3Int, int>(neighbor, heuristicDistance));
                }
            }
        }

        // Weren't able to find the path, return null
        if (!cameFrom.ContainsKey(to))
        {
            return null;
        }

        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = to;

        while (current != moving.Position)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    public IEnumerable<Vector3Int> CanAttackFrom(MapMob attacking, Vector3Int target)
    {
        // TEMPORARY: Assume everyone attacks in a perfect area around them, with no diversity in allowed patterns
        // Given this assumption, just find the PotentialAttacks from that position and return that
        // This isn't going to last, but it's handy
        return PotentialAttacks(attacking, target);
    }

    bool CanMoveInTo(MapMob moving, Vector3Int from, Vector3Int to, WorldContext worldContext)
    {
        if (!GameplayTiles.ContainsKey(to))
        {
            DebugTextLog.AddTextToLog($"Reporting that {to.x}, {to.y} is off the gameplay map and can't be moved in to");
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

    bool CanStopOn(MapMob moving, Vector3Int to, WorldContext worldContext)
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

    public IEnumerable<Vector3Int> GetNeighbors(Vector3Int point)
    {
        IEnumerable<Vector3Int> neighbors;

        if (Neighbors.TryGetValue(point, out neighbors))
        {
            return neighbors;
        }

        DebugTextLog.AddTextToLog($"Tried to get neighbors for ({point.x}, {point.y}), but the tile wasn't in the {nameof(Neighbors)} dictionary.", DebugTextLogChannel.Verbose);

        return Array.Empty<Vector3Int>();
    }

    public IEnumerable<Vector3Int> GetAllTiles()
    {
        return GameplayTiles.Keys;
    }

    public void SetTile(Vector3Int position, GameplayTile tile)
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

            List<Vector3Int> newNeighbors = new List<Vector3Int>();

            foreach (Vector3Int curNeighbor in GetPotentialNeighbors(position))
            {
                if (GameplayTiles.ContainsKey(curNeighbor))
                {
                    Neighbors[curNeighbor] = Neighbors[curNeighbor].Union(new Vector3Int[] { position });
                    newNeighbors.Add(curNeighbor);
                }
            }

            Neighbors.Add(position, newNeighbors);
        }
    }

    public void RemoveTile(Vector3Int position)
    {
        GameplayTiles.Remove(position);
        Neighbors.Remove(position);

        foreach (Vector3Int curNeighbor in GetPotentialNeighbors(position))
        {
            if (GameplayTiles.ContainsKey(curNeighbor))
            {
                Neighbors[curNeighbor] = Neighbors[curNeighbor].Except(new Vector3Int[] { position });
            }
        }
    }

    static Dictionary<Vector3Int, IEnumerable<Vector3Int>> GenerateNeighbors(Realm forRealm)
    {
        Dictionary<Vector3Int, IEnumerable<Vector3Int>> neighborsDictionary = new Dictionary<Vector3Int, IEnumerable<Vector3Int>>();

        foreach (RealmCoordinate coordinate in forRealm.RealmCoordinates)
        {
            neighborsDictionary.Add(coordinate.Position, new Vector3Int[] { });
        }

        foreach (RealmCoordinate coordinate in forRealm.RealmCoordinates)
        {
            List<Vector3Int> actualNeighbors = new List<Vector3Int>();

            foreach (Vector3Int neighbor in GetPotentialNeighbors(coordinate.Position))
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
