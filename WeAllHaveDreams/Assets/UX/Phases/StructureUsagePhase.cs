using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureUsagePhase : InputGameplayPhase
{
    public NeutralPhase NeutralPhaseInstance;
    public InputResolutionPhase InputResolutionPhaseInstance;

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    MapStructure selectedStructure { get; set; }
    List<PlayerInput> possibleInputs { get; set; }
    PlayerInput selectedInput { get; set; }

    public StructureUsagePhase StructureSelected(MapStructure structure)
    {
        selectedStructure = structure;
        possibleInputs = structure.GetPossiblePlayerInputs(WorldContextInstance).ToList();
        selectedInput = null;
        return this;
    }

    public override bool WaitingForInput => true;
    public override bool NextPhasePending => selectedInput != null;

    public override IEnumerator EnterPhase()
    {
        // TODO: Handle this navigation more elegantly!
        for (int index = 0; index < possibleInputs.Count && index < ((int)KeyCode.Y - (int)KeyCode.A); index++)
        {
            KeyCode thisChar = (KeyCode)((int)KeyCode.A + index);
            PlayerInput plan = possibleInputs[index];
            DebugTextLog.AddTextToLog($"{thisChar.ToString()}) {plan.LongTitle}");
        }

        DebugTextLog.AddTextToLog("Z) Back");

        yield break;
    }

    public override bool TryHandleKeyPress(out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        // TODO: Handle this navigation more elegantly!
        for (int index = 0; index < possibleInputs.Count && index < ((int)KeyCode.Y - (int)KeyCode.A); index ++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.A + index)))
            {
                selectedInput = possibleInputs[index];
                nextPhase = InputResolutionPhaseInstance.ResolveThis(selectedInput, NeutralPhaseInstance);
                return true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            nextPhase = NeutralPhaseInstance;
            return true;
        }

        return false;
    }
}
