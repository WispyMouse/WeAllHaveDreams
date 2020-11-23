using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobHolder : MonoBehaviour
{
    public List<MapMob> ActiveMobs { get; set; } = new List<MapMob>();

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
            return mobOnPoint;
        }

        return null;
    }

    public void MoveUnit(MapMob toMove, Vector3Int to)
    {
        MapMob onPoint;

        if ((onPoint = MobOnPoint(to)) != null && onPoint != toMove)
        {
            Debug.LogWarning($"A unit is trying to move to an occuppied tile at {{{to.x}, {to.y}, {to.z}}}");
            return;
        }

        toMove.SetPosition(to);
        DebugTextLog.AddTextToLog($"Unit <unitnamehere> moved to {{{to.x}, {to.y}, {to.z}}}");
    }

    public IEnumerator UnitEngagesUnit(MapMob engaging, MapMob defending)
    {
        decimal offensiveDamage = ProjectedDamages(engaging, defending);
        defending.HitPoints = System.Math.Max(0, defending.HitPoints - offensiveDamage);

        // TEMPORARY: This is where logic that checks to see if the unit can counter attack at this range would go
        if (defending.HitPoints > 0)
        {
            decimal returnDamage = ProjectedDamages(defending, engaging);
            engaging.HitPoints = System.Math.Max(0, engaging.HitPoints - returnDamage);
        }

        yield return new WaitForEndOfFrame();
    }

    public decimal ProjectedDamages(MapMob engaging, MapMob defending)
    {
        return engaging.CurrentAttackPower;
    }
}
