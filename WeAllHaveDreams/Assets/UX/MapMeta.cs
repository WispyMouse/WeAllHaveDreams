using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMeta : MonoBehaviour
{
    public Tilemap MetaMap;
    public MapHolder MapHolderController;

    public Tile MovementTile;

    public void ShowUnitMovementRange(MapMob toShow)
    {
        MetaMap.ClearAllTiles();

        foreach (Vector3Int tile in MapHolderController.PotentialMoves(toShow))
        {
            MetaMap.SetTile(tile, MovementTile);
        }
    }
}
