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

    public GameplayTile GetGameplayTile(MapCoordinates position) => activeMap.GetGameplayTile(position);
    public IEnumerable<MapCoordinates> GetAllTiles() => activeMap.GetAllTiles();
    public IEnumerable<MapCoordinates> GetNeighbors(MapCoordinates point) => activeMap.GetNeighbors(point);

    public IEnumerable<MapCoordinates> PotentialMoves(MapMob moving) => activeMap.PotentialMoves(moving, WorldContextInstance);
    public IEnumerable<MapCoordinates> PotentialAttacks(MapMob attacking, MapCoordinates from) => activeMap.PotentialAttacks(attacking, from);
    public List<MapCoordinates> Path(MapMob moving, MapCoordinates to) => activeMap.Path(moving, to, WorldContextInstance);
    public IEnumerable<MapCoordinates> CanHitFrom(MapMob attacking, MapCoordinates target) => activeMap.CanAttackFrom(attacking, target);

    public void LoadFromRealm(Realm toLoad)
    {
        LoadedMap.ClearAllTiles();
        activeMap = GameMap.LoadFromRealm(toLoad);
        DebugTextLog.AddTextToLog($"Loading with {toLoad.RealmCoordinates.Count()} coordinates", DebugTextLogChannel.Verbose);

        foreach (RealmCoordinate coordinate in toLoad.RealmCoordinates)
        {
            DebugTextLog.AddTextToLog($"Placing {coordinate.Tile} at {coordinate.Position.ToString()}", DebugTextLogChannel.Verbose);
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
    }

    public void SetTile(MapCoordinates position, GameplayTile toSet)
    {
        // Get all neighbors of this tile, that currently exist
        // We fetch this now in case we're clearing the tile, so it wouldn't be in the Neighbors lookup again
        IEnumerable<MapCoordinates> neighbors = GetNeighbors(position);

        activeMap.SetTile(position, toSet);
        LoadedMap.SetTile(position, toSet);

        // If this was null before, try to get neighbors again
        if (!neighbors.Any())
        {
            neighbors = GetNeighbors(position);
        }

        foreach (MapCoordinates neighbor in neighbors)
        {
            LoadedMap.RefreshTile(neighbor);
        }
    }
}
