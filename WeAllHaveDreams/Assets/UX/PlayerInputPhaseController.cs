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
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public InputGameplayPhase StartingPhase;
    InputGameplayPhase currentPhase { get; set; }
    InputGameplayPhase nextPhase;

    Coroutine phaseHandler { get; set; }
    bool shouldRefresh { get; set; }

    void Update()
    {
        if (!TurnManager.GameIsInProgress || !TurnManager.CurrentPlayer.HumanControlled)
        {
            return;
        }

        if (currentPhase == null)
        {
            return;
        }

        if (currentPhase.WaitingForInput)
        {
            HandleInput();
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
            MapCoordinates? worldpoint = LocationInput.GetHoveredTilePosition();

            // We didn't click on a position, so do nothing
            if (!worldpoint.HasValue)
            {
                return false;
            }

            MapMob mobAtPoint;
            MapStructure structureAtPoint;

            if ((mobAtPoint = WorldContextInstance.MobHolder.MobOnPoint(worldpoint.Value))
                && currentPhase.TryHandleUnitClicked(mobAtPoint, out nextPhase))
            {
                shouldRefresh = true;
            }
            else if ((structureAtPoint = WorldContextInstance.StructureHolder.StructureOnPoint(worldpoint.Value))
                && currentPhase.TryHandleStructureClicked(structureAtPoint, out nextPhase))
            {
                shouldRefresh = true;
            }
            else if (currentPhase.TryHandleTileClicked(worldpoint.Value, out nextPhase))
            {
                shouldRefresh = true;
            }

            return shouldRefresh;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ResetPhases();
            return true;
        }

        return false;
    }

    bool HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StopCoroutine(phaseHandler);
            currentPhase.EndPhase();
            TurnManager.PassTurnToNextPlayer();
            return true;
        }

        if (Input.anyKeyDown)
        {
            if (currentPhase.TryHandleKeyPress(out InputGameplayPhase nextPhaseAfterKeyPress))
            {
                nextPhase = nextPhaseAfterKeyPress;
                shouldRefresh = true;
                return true;
            }
        }

        return false;
    }

    public void ResetPhases()
    {
        if (currentPhase != null)
        {
            DebugTextLog.AddTextToLog($"Resetting phase: {currentPhase.GetType().Name}");
            currentPhase.EndPhase();
            StopCoroutine(phaseHandler);
        }

        currentPhase = StartingPhase;
        nextPhase = StartingPhase;
        shouldRefresh = false;

        phaseHandler = StartCoroutine(PhasesHandler());
    }

    IEnumerator PhasesHandler()
    {
        yield return currentPhase.EnterPhase();

        bool refresh = false;

        while (refresh == false && shouldRefresh == false)
        {
            if (currentPhase.WaitingForInput)
            {
                refresh |= shouldRefresh;
            }
            else if (currentPhase.NextPhasePending)
            {
                nextPhase = currentPhase.GetNextPhase();
                refresh = true;
            }

            yield return new WaitForEndOfFrame();
        }

        if (currentPhase != nextPhase)
        {
            currentPhase.EndPhase();
            currentPhase = nextPhase;
            DebugTextLog.AddTextToLog($"Moving to phase: {currentPhase.GetType().Name}");
        }
        else
        {
            currentPhase.UpdateAfterInput();
            DebugTextLog.AddTextToLog($"Refreshing phase: {currentPhase.GetType().Name}");
        }

        shouldRefresh = false;
        phaseHandler = StartCoroutine(PhasesHandler());
    }

    public void StopAllInputs()
    {
        StopCoroutine(phaseHandler);
    }
}
