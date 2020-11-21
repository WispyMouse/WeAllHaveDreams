using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHolder : MonoBehaviour
{
    GameMap activeMap { get; set; }

    public Tilemap LoadedMap;

    public MobHolder MobHolderController;

    void Awake()
    {
        activeMap = GameMap.InitializeMapFromTilemap(LoadedMap);
    }

    public IEnumerable<Vector3Int> PotentialMoves(MapMob moving) => activeMap.PotentialMoves(moving, MobHolderController);
}
