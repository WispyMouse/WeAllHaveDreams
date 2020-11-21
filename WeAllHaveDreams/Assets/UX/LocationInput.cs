using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocationInput : MonoBehaviour
{
    public Camera MapCamera;
    public Tilemap TerrainTilemap;
    public MapMeta MapMetaController;

    // TEMPORARY: Stored player mob so we can move them around
    public MapMob PlayerMob;

    void Update()
    {
        // TEMPORARY: When we click on a tile, teleport our unit there
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int? worldpoint = GetHoveredTilePosition();

            if (worldpoint.HasValue)
            {
                PlayerMob.SetPosition(worldpoint.Value);
                MapMetaController.ShowUnitMovementRange(PlayerMob);
            }
        }
    }

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
