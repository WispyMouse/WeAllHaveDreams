using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMobPlayerInput : PlayerInput
{
    public MapMob Moving;
    public MapCoordinates To;

    public MoveMobPlayerInput(MapMob toMove, MapCoordinates to)
    {
        Moving = toMove;
        To = to;
    }

    public override string LongTitle => $"Move {Moving.Name} to {To.ToString()}";

    public override IEnumerator Execute(WorldContext worldContext, GameplayAnimationHolder animationHolder)
    {
        if (!Moving.CanMove)
        {
            DebugTextLog.AddTextToLog("A unit tried to move, but cannot move.");
            yield break;
        }

        yield return animationHolder.MoveUnit(Moving, To);
        Moving.CanMove = false;

        yield return TurnManager.ResolveEffects();
    }
}
