using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputPhaseController : MonoBehaviour
{
    public LocationInput LocationInputController;
    public MapMeta MapMetaController;
    public MapHolder MapHolderController;

    MapMob selectedMob;

    void Update()
    {
        if (!TurnManager.CurrentPlayer.HumanControlled)
        {
            return;
        }

        HandleClick();
        HandleKeyboard();
    }

    void HandleClick()
    {
        // TEMPORARY: If we click on a tile with a unit, select that unit
        // If we click on a tile without a unit, and we have a unit selected, move the unit there
        // If we right click while we have a selected unit, clear the selection
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int? worldpoint = LocationInputController.GetHoveredTilePosition();

            // We didn't click on a position, so do nothing
            if (!worldpoint.HasValue)
            {
                return;
            }

            MapMob mobAtPoint = MapHolderController.MobOnPoint(worldpoint.Value);

            if (mobAtPoint != null)
            {
                if (mobAtPoint.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex)
                {
                    selectedMob = mobAtPoint;
                    MapMetaController.ShowUnitMovementRange(selectedMob);
                }
                
                return;
            }

            if (selectedMob != null && MapMetaController.TileIsInActiveMovementRange(worldpoint.Value))
            {
                MapHolderController.MoveUnit(selectedMob, worldpoint.Value);
                MapMetaController.ClearMetas();
                return;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (selectedMob != null)
            {
                MapMetaController.ClearMetas();
            }
        }
    }

    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TurnManager.PassTurnToNextPlayer();
        }
    }
}
