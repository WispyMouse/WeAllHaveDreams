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

    public void LoadEmptyRealm()
    {
        activeMap = GameMap.LoadEmptyRealm();
        LoadedMap.ClearAllTiles();
    }

    public IEnumerator LoadFromRealm(Realm toLoad)
    {
        try
        {
            Realm loadingRealm = toLoad;
            activeMap = GameMap.LoadFromRealm(loadingRealm);
            DebugTextLog.AddTextToLog($"Loading with {toLoad.RealmCoordinates.Count()} coordinates", DebugTextLogChannel.Verbose);

            LoadedMap.ClearAllTiles();

            foreach (RealmCoordinate coordinate in loadingRealm.RealmCoordinates)
            {
                DebugTextLog.AddTextToLog($"Placing {coordinate.Tile} at ({coordinate.Position.x}, {coordinate.Position.y})", DebugTextLogChannel.Verbose);
                LoadedMap.SetTile(coordinate.Position, TileLibrary.GetTile(coordinate.Tile));
                /*
                foreach (RealmKey curKey in toLoad.KeysAtPositions[position].OrderBy(key => (int)key.Type))
                {
                    switch (curKey.Type)
                    {
                        case RealmKeyType.Tile:
                            LoadedMap.SetTile(position, activeMap.GetGameplayTile(position));
                            break;
                        case RealmKeyType.Structure:
                            WorldContextInstance.StructureHolder.SetStructure(position, curKey.GetStructureInstance());
                            break;
                        case RealmKeyType.Mob:
                            WorldContextInstance.MobHolder.CreateNewUnit(position, curKey.GetMobPrefab());
                            break;
                        case RealmKeyType.Feature:
                            WorldContextInstance.FeatureHolder.SetFeature(position, curKey.GetFeatureInstance());
                            break;
                        case RealmKeyType.Ownership:
                            WorldContextInstance.StructureHolder.SetOwnership(position, curKey.GetTeam());

                            MapMob matchingMob = WorldContextInstance.MobHolder.MobOnPoint(position);

                            if (matchingMob != null)
                            {
                                matchingMob.SetOwnership(curKey.GetTeam());
                            }
                            break;
                    }
                }
                */
            }
        }
        catch (Exception e)
        {
            DebugTextLog.AddTextToLog($"{e.Message}, {e?.InnerException?.Message}");
            throw e;
        }

        yield break;
    }

    public void CenterCamera(Camera toCenter)
    {
        Vector3 center = new Vector3(((float)activeMap.GetAllTiles().Min(tile => tile.x) + (float)activeMap.GetAllTiles().Max(tile => tile.x)) / 2f,
            ((float)activeMap.GetAllTiles().Min(tile => tile.y) + (float)activeMap.GetAllTiles().Max(tile => tile.y)) / 2f,
            0);

        toCenter.transform.position = center + Vector3.back * 10;
    }

    public void ClearEverything()
    {
        LoadedMap.ClearAllTiles();
        WorldContextInstance.ClearEverything();
    }

    public void SetTile(Vector3Int position, GameplayTile toSet)
    {
        activeMap.SetTile(position, toSet);
        LoadedMap.SetTile(position, toSet);

        foreach (Vector3Int neighbor in GetNeighbors(position))
        {
            LoadedMap.RefreshTile(neighbor);
        }
    }
}
