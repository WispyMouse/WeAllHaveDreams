using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputResolutionPhase : InputGameplayPhase
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();
    public GameplayAnimationHolder GameplayAnimationInstance;

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

        if (!resolving.IsPossible(WorldContextInstance))
        {
            DebugTextLog.AddTextToLog("Move is not possible to execute. Canceling action.", DebugTextLogChannel.RuntimeError);

            actionResolved = true;
            yield break;
        }

        yield return resolving.Execute(WorldContextInstance, GameplayAnimationInstance);
        actionResolved = true;
    }

    public override InputGameplayPhase GetNextPhase()
    {
        return nextPhase;
    }

    public override bool WaitingForInput => false;
    public override bool NextPhasePending => actionResolved;
}
