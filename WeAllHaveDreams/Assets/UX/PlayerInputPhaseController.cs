using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Human Player's input to the game during the main game phase.
/// TODO NOTES: Lots of this is REALLY PLACEHOLDER. The logic patterns used here are not sustainable.
/// Still, they allow me to blaze through some basic control flow.
/// </summary>
public class PlayerInputPhaseController : MonoBehaviour
{
    public LocationInput LocationInputController;
    public MapMeta MapMetaController;
    public MapHolder MapHolderController;
    public MobHolder MobHolderController;

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
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int? worldpoint = LocationInputController.GetHoveredTilePosition();

            // We didn't click on a position, so do nothing
            if (!worldpoint.HasValue)
            {
                return;
            }

            if (selectedMob == null)
            {
                HandleClickWithoutSelectedMob(worldpoint.Value);
            }
            else
            {
                HandleClickWithSelectedMob(worldpoint.Value);
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

    void HandleClickWithoutSelectedMob(Vector3Int position)
    {
        MapMob mobAtPoint = MobHolderController.MobOnPoint(position);

        if (mobAtPoint != null)
        {
            if (mobAtPoint.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex
                && mobAtPoint.CanMove)
            {
                selectedMob = mobAtPoint;
                MapMetaController.ShowUnitMovementRange(selectedMob);
            }

            return;
        }
    }

    void HandleClickWithSelectedMob(Vector3Int position)
    {
        if (MobHolderController.MobOnPoint(position))
        {
            return;
        }

        selectedMob.CanMove = false;
        selectedMob.HideReminder(nameof(selectedMob.CanMove));
        MobHolderController.MoveUnit(selectedMob, position);
        MapMetaController.ClearMetas();
        selectedMob = null;
    }

    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TurnManager.PassTurnToNextPlayer();
        }
    }
}
