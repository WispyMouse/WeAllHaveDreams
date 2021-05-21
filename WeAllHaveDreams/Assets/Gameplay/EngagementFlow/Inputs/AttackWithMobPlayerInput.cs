using System.Collections;
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
        if (MoveTo.HasValue && MoveTo.Value != Attacking.Position)
        {
            yield return animationHolder.MoveUnit(Attacking, MoveTo.Value);
            Attacking.CanMove = false;
        }

        yield return animationHolder.UnitEngagesUnit(Attacking, Target);

        Attacking.CanAttack = false;

        yield return TurnManager.ResolveEffects();
    }

    public override bool IsPossible(WorldContext givenContext)
    {
        if (!Attacking.CanAttack)
        {
            return false;
        }

        if (MoveTo.HasValue && MoveTo.Value != Attacking.Position)
        {
            return false;
        }

        return true;
    }
}
