using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocationInput : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public Camera MapCamera;

    public Vector3Int? GetHoveredTilePosition()
    {
        Vector3Int worldpoint = WorldContextInstance.MapHolder.LoadedMap.WorldToCell(MapCamera.ScreenToWorldPoint(Input.mousePosition));

        if (!WorldContextInstance.MapHolder.LoadedMap.HasTile(worldpoint))
        {
            return null;
        }

        return worldpoint;
    }
}
