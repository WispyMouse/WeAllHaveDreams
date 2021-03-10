using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocationInput : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public TileCursor TileCursorInstance;
    bool tileCursorVisibility { get; set; }

    public Camera MapCamera;

    private void Update()
    {
        if (tileCursorVisibility)
        {
            Vector3Int? tileCursorPosition = GetHoveredTilePosition(false);

            if (tileCursorPosition.HasValue)
            {
                TileCursorInstance.SetPosition(tileCursorPosition.Value);
            }
        }
    }

    public Vector3Int? GetHoveredTilePosition(bool requireExistingTile = true)
    {
        Vector3Int worldpoint = WorldContextInstance.MapHolder.LoadedMap.WorldToCell(MapCamera.ScreenToWorldPoint(Input.mousePosition));

        if (requireExistingTile && !WorldContextInstance.MapHolder.LoadedMap.HasTile(worldpoint))
        {
            return null;
        }

        return worldpoint;
    }

    public void SetTileCursorVisibility(bool toVisible)
    {
        tileCursorVisibility = toVisible;
        TileCursorInstance.gameObject.SetActive(toVisible);
    }
}
