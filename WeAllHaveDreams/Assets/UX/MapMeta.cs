using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMeta : MonoBehaviour
{
    public Tilemap MetaMap;
    public MapHolder MapHolderController;

    public Tile MovementTile;

    HashSet<Vector3Int> ActiveMovementTiles { get; set; }

    public void ShowUnitMovementRange(MapMob toShow)
    {
        ClearMetas();

        foreach (Vector3Int tile in MapHolderController.PotentialMoves(toShow))
        {
            MetaMap.SetTile(tile, MovementTile);
            ActiveMovementTiles.Add(tile);
        }
    }

    public bool TileIsInActiveMovementRange(Vector3Int position)
    {
        return ActiveMovementTiles != null
            && ActiveMovementTiles.Contains(position);
    }

    public void ClearMetas()
    {
        MetaMap.ClearAllTiles();
        ActiveMovementTiles = new HashSet<Vector3Int>();
    }
}
