using System.Collections;
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

    public override IEnumerator Execute(MapHolder mapHolder, MobHolder mobHolder)
    {
        if (!Attacking.CanAttack)
        {
            DebugTextLog.AddTextToLog("A unit tried to attack, but cannot attack");
            yield break;
        }

        if (MoveTo.HasValue)
        {
            if (!Attacking.CanMove)
            {
                DebugTextLog.AddTextToLog("A unit tried to move and attack, but cannot move");
                yield break;
            }

            yield return mobHolder.MoveUnit(Attacking, MoveTo.Value);
            Attacking.CanMove = false;
        }

        yield return mobHolder.UnitEngagesUnit(Attacking, Target);
        Attacking.CanAttack = false;
    }
}
