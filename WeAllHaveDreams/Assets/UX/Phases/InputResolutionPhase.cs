using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputResolutionPhase : InputGameplayPhase
{
    public WorldContext WorldContextInstance;

    PlayerInput resolving { get; set; }
    InputGameplayPhase nextPhase { get; set; }
    bool actionResolved { get; set; } = false;

    public InputResolutionPhase ResolveThis(PlayerInput toResolve, InputGameplayPhase thenPhase)
    {
        resolving = toResolve;
        nextPhase = thenPhase;
        actionResolved = false;
        return this;
    }

    public override IEnumerator EnterPhase()
    {
        actionResolved = false;
        yield return resolving.Execute(WorldContextInstance);
        actionResolved = true;
    }

    public override InputGameplayPhase GetNextPhase()
    {
        return nextPhase;
    }

    public override bool WaitingForInput => false;
    public override bool NextPhasePending => actionResolved;
}
