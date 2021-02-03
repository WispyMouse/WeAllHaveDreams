﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWithMobInput : PlayerInput
{
    public MapMob Attacking;
    public MapMob Target;
    public Vector3Int? MoveTo;

    public AttackWithMobInput(MapMob attacking, MapMob target)
    {
        Attacking = attacking;
        Target = target;
        MoveTo = null; // Explicitly not moving
    }

    public AttackWithMobInput(MapMob attacking, MapMob target, Vector3Int moveTo)
    {
        Attacking = attacking;
        Target = target;
        MoveTo = moveTo;
    }

    public override string LongTitle => $"Attack with {Attacking.name} at {Target.name}" + (MoveTo.HasValue ? $" after moving to ({MoveTo.Value.x}, {MoveTo.Value.y})" : "");

    public override IEnumerator Execute(WorldContext worldContext)
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

            yield return worldContext.MobHolder.MoveUnit(Attacking, MoveTo.Value);
            Attacking.CanMove = false;
        }

        yield return worldContext.MobHolder.UnitEngagesUnit(Attacking, Target);

        Attacking.CanAttack = false;

        yield return TurnManager.ResolveEffects();
    }
}
