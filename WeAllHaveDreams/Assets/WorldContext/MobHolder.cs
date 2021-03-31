using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobHolder : MonoBehaviour
{
    public List<MapMob> ActiveMobs { get; set; } = new List<MapMob>();

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

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
        return WorldContextInstance.MapHolder.PotentialAttacks(possibleAttacker, attackerPosition).Contains(defender.Position);
    }

    public void RemoveMob(MapMob toRemove)
    {
        DebugTextLog.AddTextToLog($"Removing {toRemove.Name} from the map");
        ActiveMobs.Remove(toRemove);
        Destroy(toRemove.gameObject);
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
        newMob.CalculateStandingStatAdjustments(WorldContextInstance.FeatureHolder.FeatureOnPoint(location));
    }

    public void CreateNewUnit(MobMapData data)
    {
        CreateNewUnit(data.Position, MobLibrary.GetMob(data.MobName), data.Ownership);
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
