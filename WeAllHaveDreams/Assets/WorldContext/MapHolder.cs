using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHolder : MonoBehaviour
{
    GameMap activeMap { get; set; }

    public Tilemap LoadedMap;

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public GameplayTile GetGameplayTile(Vector3Int position) => activeMap.GetGameplayTile(position);
    public IEnumerable<Vector3Int> GetAllTiles() => activeMap.GetAllTiles();
    public IEnumerable<Vector3Int> GetNeighbors(Vector3Int point) => activeMap.GetNeighbors(point);

    public IEnumerable<Vector3Int> PotentialMoves(MapMob moving) => activeMap.PotentialMoves(moving, WorldContextInstance);
    public IEnumerable<Vector3Int> PotentialAttacks(MapMob attacking, Vector3Int from) => activeMap.PotentialAttacks(attacking, from);
    public List<Vector3Int> Path(MapMob moving, Vector3Int to) => activeMap.Path(moving, to, WorldContextInstance);
    public IEnumerable<Vector3Int> CanHitFrom(MapMob attacking, Vector3Int target) => activeMap.CanAttackFrom(attacking, target);

    public void LoadFromRealm(Realm toLoad)
    {
        LoadedMap.ClearAllTiles();
        activeMap = GameMap.LoadFromRealm(toLoad);
        DebugTextLog.AddTextToLog($"Loading with {toLoad.RealmCoordinates.Count()} coordinates", DebugTextLogChannel.Verbose);

        foreach (RealmCoordinate coordinate in toLoad.RealmCoordinates)
        {
            DebugTextLog.AddTextToLog($"Placing {coordinate.Tile} at ({coordinate.Position.x}, {coordinate.Position.y})", DebugTextLogChannel.Verbose);
            LoadedMap.SetTile(coordinate.Position, TileLibrary.GetTile(coordinate.Tile));
        }

        LoadedMap.RefreshAllTiles();
    }

    public void LoadEmptyRealm()
    {
        activeMap = GameMap.LoadEmptyRealm();
        LoadedMap.ClearAllTiles();
    }

    public void ClearEverything()
    {
        LoadedMap.ClearAllTiles();
        WorldContextInstance.ClearEverything();
    }

    public void SetTile(Vector3Int position, GameplayTile toSet)
    {
        // Get all neighbors of this tile, that currently exist
        // We fetch this now in case we're clearing the tile, so it wouldn't be in the Neighbors lookup again
        IEnumerable<Vector3Int> neighbors = GetNeighbors(position);

        activeMap.SetTile(position, toSet);
        LoadedMap.SetTile(position, toSet);

        // If this was null before, try to get neighbors again
        if (!neighbors.Any())
        {
            neighbors = GetNeighbors(position);
        }

        foreach (Vector3Int neighbor in neighbors)
        {
            LoadedMap.RefreshTile(neighbor);
        }
    }

    public void SetOwnership(Vector3Int position, int? value)
    {
        MapStructure structure;
        if (structure = WorldContextInstance.StructureHolder.StructureOnPoint(position))
        {
            structure.SetOwnership(value);
        }

        MapMob mob;
        if (mob = WorldContextInstance.MobHolder.MobOnPoint(position))
        {
            mob.SetOwnership(value.Value);
        }
    }
}
