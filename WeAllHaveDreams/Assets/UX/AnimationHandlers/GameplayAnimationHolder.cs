using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayAnimationHolder : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public MovementHandler MovementHandlerInstance;
    public AttackHandler AttackAnimationHandlerInstance;

    public IEnumerator MoveUnit(MapMob toMove, MapCoordinates to)
    {
        MapMob onPoint;

        if ((onPoint = WorldContextInstance.MobHolder.MobOnPoint(to)) != null && onPoint != toMove)
        {
            DebugTextLog.AddTextToLog($"A unit is trying to move to an occuppied tile at {to.ToString()}", DebugTextLogChannel.RuntimeError);
            yield break;
        }

        toMove.RestingPosition = toMove.Position;
        WorldContextInstance.StructureHolder.MobRemovedFromPoint(toMove.Position);
        yield return MovementHandlerInstance.UnitWalks(toMove, to);

        toMove.SetPosition(to);
        DebugTextLog.AddTextToLog($"Unit {toMove.Name} moved to {to.ToString()}", DebugTextLogChannel.Verbose);

        toMove.CalculateStandingStatAdjustments(WorldContextInstance.FeatureHolder.FeatureOnPoint(to));
    }

    public IEnumerator UnitEngagesUnit(MapMob engaging, MapMob defending)
    {
        decimal offensiveDamage = WorldContextInstance.MobHolder.ProjectedDamages(engaging, defending);

        yield return AttackAnimationHandlerInstance.UnitAttacksUnit(engaging, defending, new System.Action(() =>
        {
            defending.HitPoints = System.Math.Max(0, defending.HitPoints - offensiveDamage);
            DebugTextLog.AddTextToLog($"{engaging.Name} deals {offensiveDamage} damage to {defending.Name}! ({defending.HitPoints} remaining)", DebugTextLogChannel.Gameplay);
        }));

        if (defending.HitPoints > 0)
        {
            // if we're out of range, we can't counter
            if (!WorldContextInstance.MobHolder.CanAttackFromPosition(defending, engaging, defending.Position))
            {
                yield break;
            }

            decimal returnDamage = WorldContextInstance.MobHolder.ProjectedDamages(defending, engaging);

            yield return AttackAnimationHandlerInstance.UnitAttacksUnit(defending, engaging, new System.Action(() =>
            {
                engaging.HitPoints = System.Math.Max(0, engaging.HitPoints - returnDamage);
                DebugTextLog.AddTextToLog($"{defending.Name} counters with {returnDamage} damage to {engaging.Name}! ({engaging.HitPoints} remaining)", DebugTextLogChannel.Gameplay);
            }));
        }
    }
}
