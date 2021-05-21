using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseAbilityPhase : InputGameplayPhase
{
    public NeutralPhase NeutralPhaseInstance;
    public InputResolutionPhase InputResolutionPhaseInstance;

    MapMob selectedUnit;
    MobConfigurationAbility selectedAbility;

    PlayerInput selectedInput { get; set; }
    SortedDictionary<KeyCode, PlayerInput> possibleInputs { get; set; }

    public UseAbilityPhase AbilitySelected(MapMob mob, MobConfigurationAbility ability)
    {
        selectedUnit = mob;
        selectedAbility = ability;

        List<PlayerInput> inputs = ability.GetPossiblePlayerInputs(mob).ToList();
        possibleInputs = SortOptionsInToDictionary(inputs);

        selectedInput = null;
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

        DebugTextLog.AddTextToLog("Z) Back");

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
