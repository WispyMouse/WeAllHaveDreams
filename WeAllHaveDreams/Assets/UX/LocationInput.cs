using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocationInput : MonoBehaviour
{
    public Camera MapCamera;
    public Tilemap TerrainTilemap;

    public Vector3Int? GetHoveredTilePosition()
    {
        Vector3Int worldpoint = TerrainTilemap.WorldToCell(MapCamera.ScreenToWorldPoint(Input.mousePosition));

        if (!TerrainTilemap.HasTile(worldpoint))
        {
            return null;
        }

        return worldpoint;
    }
}
