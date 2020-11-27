using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureUsagePhase : InputGameplayPhase
{
    public NeutralPhase NeutralPhaseInstance;
    public InputResolutionPhase InputResolutionPhaseInstance;

    public MobHolder MobHolderInstance;

    MapStructure selectedStructure { get; set; }

    public StructureUsagePhase StructureSelected(MapStructure structure)
    {
        selectedStructure = structure;
        return this;
    }

    public override bool WaitingForInput => false;
    public override bool NextPhasePending => true;
    public override InputGameplayPhase GetNextPhase()
    {
        PlayerInput input = selectedStructure.DoLazyBuildingThing(MobHolderInstance);

        if (input != null)
        {
            return InputResolutionPhaseInstance.ResolveThis(input, NeutralPhaseInstance);
        }

        return NeutralPhaseInstance;
    }
}
