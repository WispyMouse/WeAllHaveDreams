using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHolder : MonoBehaviour
{
    GameMap activeMap { get; set; }

    public Tilemap LoadedMap;

    void Start()
    {
        activeMap = GameMap.InitializeMapFromTilemap(LoadedMap);
    }
}
