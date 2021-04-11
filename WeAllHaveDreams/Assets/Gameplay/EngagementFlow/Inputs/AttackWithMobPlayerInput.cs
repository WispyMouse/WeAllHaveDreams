﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWithMobInput : PlayerInput
{
    public MapMob Attacking;
    public MapMob Target;
    public MapCoordinates? MoveTo;

    public AttackWithMobInput(MapMob attacking, MapMob target)
    {
        Attacking = attacking;
        Target = target;
        MoveTo = null; // Explicitly not moving
    }

    public AttackWithMobInput(MapMob attacking, MapMob target, MapCoordinates moveTo)
    {
        Attacking = attacking;
        Target = target;
        MoveTo = moveTo;
    }

    public override string LongTitle => $"Attack with {Attacking.Name} at {Target.Name}" + (MoveTo.HasValue ? $" after moving to ({MoveTo.Value.ToString()})" : "");

    public override IEnumerator Execute(WorldContext worldContext, GameplayAnimationHolder animationHolder)
    {
        if (!Attacking.CanAttack)
        {
            DebugTextLog.AddTextToLog("A unit tried to attack, but cannot attack");
            yield break;
        }

        if (MoveTo.HasValue && MoveTo.Value != Attacking.Position)
        {
            if (!Attacking.CanMove)
            {
                DebugTextLog.AddTextToLog("A unit tried to move and attack, but cannot move");
                yield break;
            }

            yield return animationHolder.MoveUnit(Attacking, MoveTo.Value);
            Attacking.CanMove = false;
        }

        yield return animationHolder.UnitEngagesUnit(Attacking, Target);

        Attacking.CanAttack = false;

        yield return TurnManager.ResolveEffects();
    }
}
