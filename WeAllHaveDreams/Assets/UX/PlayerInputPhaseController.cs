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

    public InputGameplayPhase StartingPhase;
    InputGameplayPhase currentPhase { get; set; }
    InputGameplayPhase nextPhase { get; set; }

    private void Start()
    {
        currentPhase = StartingPhase;
        currentPhase.EnterPhase();
    }

    void Update()
    {
        if (!TurnManager.CurrentPlayer.HumanControlled)
        {
            return;
        }

        bool refresh = false;

        if (currentPhase.WaitingForInput)
        {
            refresh = HandleInput();
        }
        else if (currentPhase.NextPhasePending)
        {
            nextPhase = currentPhase.GetNextPhase();
            refresh = true;
        }

        if (refresh)
        {
            if (currentPhase != nextPhase)
            {
                currentPhase.EndPhase();
                currentPhase = nextPhase;
                currentPhase.EnterPhase();

                DebugTextLog.AddTextToLog($"Moving to phase: {currentPhase.GetType().Name}");
            }
            else
            {
                currentPhase.UpdateAfterInput();
                DebugTextLog.AddTextToLog($"Refreshing phase: {currentPhase.GetType().Name}");
            }
        }
    }

    bool HandleInput()
    {
        bool inputHandled = HandleClick();
        inputHandled |= HandleKeyboard();
        return inputHandled;
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

            MapMob mobAtPoint = MobHolderController.MobOnPoint(worldpoint.Value);

            if (mobAtPoint)
            {
                nextPhase = currentPhase.UnitClicked(mobAtPoint);
            }
            else
            {
                nextPhase = currentPhase.TileClicked(worldpoint.Value);
            }

            return true;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            nextPhase = StartingPhase;
            return true;
        }

        return false;
    }

    bool HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TurnManager.PassTurnToNextPlayer();
            return true;
        }

        if (Input.anyKeyDown)
        {
            if (currentPhase.TryHandleKeyPress(out InputGameplayPhase nextPhaseAfterKeyPress))
            {
                nextPhase = nextPhaseAfterKeyPress;
                return true;
            }
        }

        return false;
    }

    public void ResetPhases()
    {
        if (currentPhase != null)
        {
            currentPhase.EndPhase();
        }

        currentPhase = StartingPhase;
        currentPhase.EnterPhase();
        nextPhase = currentPhase;
        DebugTextLog.AddTextToLog($"Resetting phase: {currentPhase.GetType().Name}");
    }
}
