using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorRuntimeController : MonoBehaviour
{
    public LocationInput LocationInputController;

    private void Update()
    {
        HandleClick();
    }

    bool HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int? worldpoint = LocationInputController.GetHoveredTilePosition();

            // We didn't click on a position, so do nothing
            if (!worldpoint.HasValue)
            {
                return false;
            }

            ApplyTilePalette(worldpoint.Value);
            return true;
        }

        return false;
    }

    void ApplyTilePalette(Vector3Int worldPoint)
    {

    }
}
