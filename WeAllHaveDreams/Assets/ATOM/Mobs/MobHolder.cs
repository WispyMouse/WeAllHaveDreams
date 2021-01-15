﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobHolder : MonoBehaviour
{
    public List<MapMob> ActiveMobs { get; set; } = new List<MapMob>();

    public MapHolder MapHolderInstance;
    public StructureHolder StructureHolderInstance;

    public MovementHandler MovementHandlerInstance;
    public AttackHandler AttackAnimationHandlerInstance;

    public Transform MobParent;

    private void Awake()
    {
        LoadAllMobsFromScene();
    }

    public IEnumerable<MapMob> MobsOnTeam(int teamIndex)
    {
        return ActiveMobs.Where(mob => mob.PlayerSideIndex == teamIndex);
    }

    public void LoadAllMobsFromScene()
    {
        ActiveMobs = new List<MapMob>();

        foreach (MapMob curMob in GameObject.FindObjectsOfType<MapMob>())
        {
            curMob.SettleIntoGrid();

            if (ActiveMobs.Any(mob => mob.Position == curMob.Position))
            {
                Debug.LogWarning($"Multiple mobs are on the same position: {{{curMob.Position.x}, {curMob.Position.y}, {curMob.Position.z}}}");
            }

            ActiveMobs.Add(curMob);
        }
    }

    public MapMob MobOnPoint(Vector3Int position)
    {
        MapMob mobOnPoint;

        if ((mobOnPoint = ActiveMobs.FirstOrDefault(mob => mob.Position == position)) != null)
        {
            // DebugTextLog.AddTextToLog($"Reporting that there is a mob at {position.x}, {position.y}");
            return mobOnPoint;
        }

        return null;
    }

    public IEnumerator MoveUnit(MapMob toMove, Vector3Int to)
    {
        MapMob onPoint;

        if ((onPoint = MobOnPoint(to)) != null && onPoint != toMove)
        {
            DebugTextLog.AddTextToLog($"A unit is trying to move to an occuppied tile at {{{to.x}, {to.y}, {to.z}}}");
            yield break;
        }

        StructureHolderInstance.MobRemovedFromPoint(toMove.Position);
        yield return MovementHandlerInstance.UnitWalks(toMove, to);

        toMove.SetPosition(to);
        DebugTextLog.AddTextToLog($"Unit {toMove.Name} moved to {{{to.x}, {to.y}, {to.z}}}");
    }

    public IEnumerator UnitEngagesUnit(MapMob engaging, MapMob defending)
    {
        decimal offensiveDamage = ProjectedDamages(engaging, defending);

        yield return AttackAnimationHandlerInstance.UnitAttacksUnit(engaging, defending, new System.Action(() =>
        {
            defending.HitPoints = System.Math.Max(0, defending.HitPoints - offensiveDamage);
            DebugTextLog.AddTextToLog($"{engaging.Name} deals {offensiveDamage} damage to {defending.Name}! ({defending.HitPoints} remaining)");
        }));

        if (defending.HitPoints > 0)
        {
            // if we're out of range, we can't counter
            if (!CanAttackFromPosition(defending, engaging, defending.Position))
            {
                yield break;
            }

            decimal returnDamage = ProjectedDamages(defending, engaging);

            yield return AttackAnimationHandlerInstance.UnitAttacksUnit(defending, engaging, new System.Action(() =>
            {
                engaging.HitPoints = System.Math.Max(0, engaging.HitPoints - returnDamage);
                DebugTextLog.AddTextToLog($"{defending.Name} counters with {returnDamage} damage to {engaging.Name}! ({engaging.HitPoints} remaining)");
            }));
        }
    }

    public decimal ProjectedDamages(MapMob engaging, MapMob defending)
    {
        return engaging.CurrentAttackPower * defending.DamageReductionRatio;
    }

    public decimal ProjectedDamages(MapMob engaging, MapMob defending, decimal overrideEngagingHealth)
    {
        return engaging.AttackPowerAtHitPoints(overrideEngagingHealth) * defending.DamageReductionRatio;
    }

    public bool CanAttackFromPosition(MapMob possibleAttacker, MapMob defender, Vector3Int attackerPosition)
    {
        return MapHolderInstance.PotentialAttacks(possibleAttacker, attackerPosition).Contains(defender.Position);
    }

    public IEnumerator RemoveMob(MapMob toRemove)
    {
        DebugTextLog.AddTextToLog($"Removing {toRemove.Name} from the map");
        ActiveMobs.Remove(toRemove);
        Destroy(toRemove.gameObject);
        yield return new WaitForEndOfFrame();
    }

    public MapMob CreateNewUnit(Vector3Int location, MapMob prefab)
    {
        if (ActiveMobs.Any(mob => mob.Position == location))
        {
            DebugTextLog.AddTextToLog("Tried to create another unit in an occuppied tile.");
            return null;
        }

        MapMob newMob = Instantiate(prefab, MobParent);
        newMob.LoadFromConfiguration(prefab.Configuration);
        newMob.SetPosition(location);
        newMob.SetUnitVisuals();
        newMob.ExhaustAllOptions();
        newMob.gameObject.SetActive(true);
        ActiveMobs.Add(newMob);
        return newMob;
    }

    public void CreateNewUnit(Vector3Int location, MapMob prefab, int teamIndex)
    {
        MapMob newMob = CreateNewUnit(location, prefab);
        newMob.SetOwnership(teamIndex);
    }

    public void ClearAllMobs()
    {
        foreach (MapMob curMob in GameObject.FindObjectsOfType<MapMob>())
        {
            Destroy(curMob.gameObject);
        }

        ActiveMobs = new List<MapMob>();
    }
}
