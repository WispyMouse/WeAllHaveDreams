using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureUsagePhase : InputGameplayPhase
{
    public NeutralPhase NeutralPhaseInstance;
    public MobHolder MobHolderInstance;

    MapStructure selectedStructure { get; set; }

    public StructureUsagePhase StructureSelected(MapStructure structure)
    {
        selectedStructure = structure;
        return this;
    }

    public override IEnumerator EnterPhase()
    {
        selectedStructure.DoLazyBuildingThing(MobHolderInstance);
        yield break;
    }

    public override bool WaitingForInput => false;
    public override bool NextPhasePending => true;
    public override InputGameplayPhase GetNextPhase()
    {
        return NeutralPhaseInstance;
    }
}
