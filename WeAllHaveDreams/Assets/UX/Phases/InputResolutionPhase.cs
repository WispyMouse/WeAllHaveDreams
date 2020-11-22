using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputResolutionPhase : InputGameplayPhase
{
    public MapHolder MapHolderInstance;
    public MobHolder MobHolderInstance;

    PlayerInput resolving { get; set; }
    InputGameplayPhase nextPhase { get; set; }

    public InputResolutionPhase ResolveThis(PlayerInput toResolve, InputGameplayPhase thenPhase)
    {
        resolving = toResolve;
        nextPhase = thenPhase;
        return this;
    }

    public override void EnterPhase()
    {
        resolving.Execute(MapHolderInstance, MobHolderInstance);
    }

    public override InputGameplayPhase GetNextPhase()
    {
        return nextPhase;
    }

    public override bool WaitingForInput => false;
    public override bool NextPhasePending => true;
}
