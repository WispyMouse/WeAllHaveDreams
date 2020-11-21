using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHolder : MonoBehaviour
{
    GameMap activeMap { get; set; }

    public Tilemap LoadedMap;

    void Awake()
    {
        activeMap = GameMap.InitializeMapFromTilemap(LoadedMap);
        activeMap.LoadAllMobsFromScene();
    }

    public void MoveUnit(MapMob toMove, Vector3Int to)
    {
        activeMap.ClearUnitAtPosition(toMove.Position);
        activeMap.SetUnitAtPosition(toMove, to);
    }

    public IEnumerable<Vector3Int> PotentialMoves(MapMob moving) => activeMap.PotentialMoves(moving);
    public MapMob MobOnPoint(Vector3Int point) => activeMap.MobOnPoint(point);
}
