using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobHolder : MonoBehaviour
{
    public List<MapMob> ActiveMobs { get; set; } = new List<MapMob>();

    public MovementHandler MovementHandlerInstance;
    public AttackHandler AttackAnimationHandlerInstance;

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
            Debug.LogWarning($"A unit is trying to move to an occuppied tile at {{{to.x}, {to.y}, {to.z}}}");
            yield break;
        }

        yield return MovementHandlerInstance.UnitWalks(toMove, to);

        toMove.SetPosition(to);
        DebugTextLog.AddTextToLog($"Unit <unitnamehere> moved to {{{to.x}, {to.y}, {to.z}}}");
    }

    public IEnumerator UnitEngagesUnit(MapMob engaging, MapMob defending)
    {
        decimal offensiveDamage = ProjectedDamages(engaging, defending);

        yield return AttackAnimationHandlerInstance.UnitAttacksUnit(engaging, defending, new System.Action(() =>
        {
            defending.HitPoints = System.Math.Max(0, defending.HitPoints - offensiveDamage);
            DebugTextLog.AddTextToLog($"<mobname> deals {offensiveDamage} damage to <mobname>! ({defending.HitPoints} remaining)");
        }));

        // TEMPORARY: This is where logic that checks to see if the unit can counter attack at this range would go
        if (defending != null && defending.HitPoints > 0)
        {
            decimal returnDamage = ProjectedDamages(defending, engaging);

            yield return AttackAnimationHandlerInstance.UnitAttacksUnit(defending, engaging, new System.Action(() =>
            {
                engaging.HitPoints = System.Math.Max(0, engaging.HitPoints - returnDamage);
                DebugTextLog.AddTextToLog($"<mobname> counters with {returnDamage} damage to <mobname>! ({engaging.HitPoints} remaining)");
            }));
        }
    }

    public decimal ProjectedDamages(MapMob engaging, MapMob defending)
    {
        return engaging.CurrentAttackPower;
    }

    public IEnumerator RemoveMob(MapMob toRemove)
    {
        DebugTextLog.AddTextToLog("Removing <mobname> from the map");
        ActiveMobs.Remove(toRemove);
        Destroy(toRemove.gameObject);
        yield return new WaitForEndOfFrame();
    }

    public void CreateNewUnit(MapMob prefab, int teamIndex, Vector3Int location)
    {
        if (ActiveMobs.Any(mob => mob.Position == location))
        {
            DebugTextLog.AddTextToLog("Tried to create another unit in an occuppied tile.");
            return;
        }

        MapMob newMob = Instantiate(prefab);
        newMob.SetPosition(location);
        newMob.PlayerSideIndex = teamIndex;
        newMob.SetUnitVisuals();
        ActiveMobs.Add(newMob);
    }
}
