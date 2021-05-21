using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Phase for using the services of a structure.
/// </summary>
public class StructureUsagePhase : InputGameplayPhase
{
    public NeutralPhase NeutralPhaseInstance;
    public InputResolutionPhase InputResolutionPhaseInstance;

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    MapStructure selectedStructure { get; set; }
    PlayerInput selectedInput { get; set; }

    SortedDictionary<KeyCode, PlayerInput> possibleInputs { get; set; }

    public StructureUsagePhase StructureSelected(MapStructure structure)
    {
        selectedStructure = structure;
        selectedInput = null;
        possibleInputs = new SortedDictionary<KeyCode, PlayerInput>();

        List<PlayerInput> inputs = structure.GetPossiblePlayerInputs(WorldContextInstance).ToList();
        possibleInputs = SortOptionsInToDictionary(inputs);

        return this;
    }

    public override bool WaitingForInput => true;
    public override bool NextPhasePending => selectedInput != null;

    public override IEnumerator EnterPhase()
    {
        foreach (KeyValuePair<KeyCode, PlayerInput> input in possibleInputs)
        {
            DebugTextLog.AddTextToLog($"{input.Key.ToString()}) {input.Value.LongTitle}");
        }

        DebugTextLog.AddTextToLog("Z) Back", DebugTextLogChannel.DebugOperationInputInstructions);

        yield break;
    }

    public override bool TryHandleKeyPress(out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        foreach (KeyValuePair<KeyCode, PlayerInput> input in possibleInputs)
        {
            if (Input.GetKeyDown(input.Key))
            {
                selectedInput = input.Value;
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
