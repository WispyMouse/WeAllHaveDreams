using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocationInput : SingletonBase<LocationInput>
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public TileCursor TileCursorInstance;
    bool tileCursorVisibility { get; set; }

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

    public static Vector3Int? GetHoveredTilePosition(bool requireExistingTile = true)
    {
        Vector3Int worldpoint = Singleton.WorldContextInstance.MapHolder.LoadedMap.WorldToCell(CameraController.ScreenToWorldPoint(Input.mousePosition));

        if (requireExistingTile && !Singleton.WorldContextInstance.MapHolder.LoadedMap.HasTile(worldpoint))
        {
            return null;
        }

        return worldpoint;
    }

    public static void SetTileCursorVisibility(bool toVisible)
    {
        Singleton.tileCursorVisibility = toVisible;
        Singleton.TileCursorInstance.gameObject.SetActive(toVisible);
    }
}
