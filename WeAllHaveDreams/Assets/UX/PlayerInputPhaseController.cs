using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputPhaseController : MonoBehaviour
{
    public LocationInput LocationInputController;
    public MapMeta MapMetaController;

    // TEMPORARY: Stored player mob so we can move them around
    public MapMob PlayerMob;

    private void Start()
    {
        MapMetaController.ShowUnitMovementRange(PlayerMob);
    }

    void Update()
    {
        // TEMPORARY: When we click on a tile, teleport our unit there
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int? worldpoint = LocationInputController.GetHoveredTilePosition();

            if (worldpoint.HasValue && MapMetaController.TileIsInActiveMovementRange(worldpoint.Value))
            {
                PlayerMob.SetPosition(worldpoint.Value);
                MapMetaController.ShowUnitMovementRange(PlayerMob);
            }
        }
    }
}
