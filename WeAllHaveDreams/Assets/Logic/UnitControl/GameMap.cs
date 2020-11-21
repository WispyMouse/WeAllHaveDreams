using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMap
{
    Dictionary<Vector3Int, IEnumerable<Vector3Int>> Neighbors { get; set; }

    public static GameMap InitializeMapFromTilemap(Tilemap tileMap)
    {
        GameMap newMap = new GameMap();

        var neighbors = new Dictionary<Vector3Int, IEnumerable<Vector3Int>>();

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
            }

            neighbors.Add(thisTile, actualNeighbors);
        }

        newMap.Neighbors = neighbors;

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

        return possibleVisits.Keys;
    }

    bool CanMoveInTo(MapMob moving, Vector3Int from, Vector3Int to, MobHolder mobHolder)
    {
        MapMob mobOnPoint;

        if ((mobOnPoint = mobHolder.MobOnPoint(to)) != null && mobOnPoint.PlayerSideIndex != moving.PlayerSideIndex)
        {
            return false;
        }

        return true;
    }
}
