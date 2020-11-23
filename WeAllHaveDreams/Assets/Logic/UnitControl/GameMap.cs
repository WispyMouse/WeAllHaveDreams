using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMap
{
    Dictionary<Vector3Int, IEnumerable<Vector3Int>> Neighbors { get; set; }
    Dictionary<Vector3Int, GameplayTile> GameplayTiles { get; set; }

    public GameplayTile GetGameplayTile(Vector3Int position)
    {
        return GameplayTiles[position];
    }

    public static GameMap InitializeMapFromTilemap(Tilemap tileMap)
    {
        GameMap newMap = new GameMap();

        var neighbors = new Dictionary<Vector3Int, IEnumerable<Vector3Int>>();
        var gameplayTiles = new Dictionary<Vector3Int, GameplayTile>();

        var traveled = new HashSet<Vector3Int>() { Vector3Int.zero };
        var frontier = new HashSet<Vector3Int>() { Vector3Int.zero };

        while (frontier.Any())
        {
            Vector3Int thisTile = frontier.First();
            frontier.Remove(thisTile);

            HashSet<Vector3Int> actualNeighbors = new HashSet<Vector3Int>();

            foreach (Vector3Int curPotentialNeighbor in GetPotentialNeighbors(thisTile))
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

    static Vector3Int[] GetPotentialNeighbors(Vector3Int center)
    {
        return new Vector3Int[]
        {
            center + Vector3Int.right,
            center + Vector3Int.up,
            center + Vector3Int.left,
            center + Vector3Int.down
        };
    }

    public IEnumerable<Vector3Int> PotentialMoves(MapMob movingMob, MobHolder mobHolderController)
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

            foreach (Vector3Int neighbor in Neighbors[thisTile.Key])
            {
                int totalCost = thisTile.Value + 1; // TEMPORARY: This will eventually consider the movement cost of the tile we're moving on to

                if (!CanMoveInTo(movingMob, thisTile.Key, neighbor, mobHolderController))
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
            if (CanStopOn(movingMob, position, mobHolderController))
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

            foreach (Vector3Int neighbor in Neighbors[thisTile.Key])
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

    public List<Vector3Int> Path(MapMob moving, Vector3Int to, MobHolder mobHolder)
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

            foreach (Vector3Int neighbor in Neighbors[positionValue])
            {
                // Can't move here, don't consider it
                if (!CanMoveInTo(moving, positionValue, neighbor, mobHolder))
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
                        frontier.Add(new Tuple<Vector3Int, int>(positionValue, heuristicDistance));
                    }
                }
                else
                {
                    costSoFar.Add(neighbor, newCost);
                    cameFrom.Add(neighbor, positionValue);
                    frontier.Add(new Tuple<Vector3Int, int>(positionValue, heuristicDistance));
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

    bool CanMoveInTo(MapMob moving, Vector3Int from, Vector3Int to, MobHolder mobHolder)
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

        if ((mobOnPoint = mobHolder.MobOnPoint(to)) != null && mobOnPoint.PlayerSideIndex != moving.PlayerSideIndex)
        {
            return false;
        }

        return true;
    }

    bool CanStopOn(MapMob moving, Vector3Int to, MobHolder mobHolder)
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

        if ((mobOnPoint = mobHolder.MobOnPoint(to)) != null && mobOnPoint != moving)
        {
            return false;
        }

        return true;
    }
}
