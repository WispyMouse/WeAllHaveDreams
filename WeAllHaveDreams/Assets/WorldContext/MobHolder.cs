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
                Debug.LogWarning($"Multiple mobs are on the same position: {curMob.Position.ToString()}");
            }

            ActiveMobs.Add(curMob);
        }
    }

    public MapMob MobOnPoint(MapCoordinates position)
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
        return ProjectedDamages(engaging, defending, engaging.HitPoints);
    }

    public decimal ProjectedDamages(MapMob engaging, MapMob defending, decimal overrideEngagingHealth)
    {
        // We start by taking the raw offensive output of this mob
        decimal damage = engaging.AttackPowerAtHitPoints(overrideEngagingHealth);
        DebugTextLog.AddTextToLog($"Base damage: {damage}", DebugTextLogChannel.Verbose);

        // Then reduce by the defending unit's defensive ratio
        damage *= defending.DamageReductionRatio;
        DebugTextLog.AddTextToLog($"Defending unit defensive ratio: {defending.DamageReductionRatio}", DebugTextLogChannel.Verbose);

        MapStructure onStructure = WorldContextInstance.StructureHolder.StructureOnPoint(defending.Position);
        DefensiveAttributes defense = null;
        if (onStructure != null)
        {
            defense = onStructure.Configuration?.Defenses?
                .OrderByDescending(def => def.Priority)
                .Where(def => def.TagsApply(defending.Tags))
                .FirstOrDefault();
        }

        if (defense == null)
        {
            GameplayTile tile = WorldContextInstance.MapHolder.GetGameplayTile(defending.Position);
            defense = tile.Configuration?.Defenses?
                .OrderByDescending(def => def.Priority)
                .Where(def => def.TagsApply(defending.Tags))
                .FirstOrDefault();
        }

        if (defense != null)
        {
            decimal defensiveRatio = defense.DefensiveRatio;
            damage *= defensiveRatio;
            DebugTextLog.AddTextToLog($"Defending unit structure defensive ratio: {defensiveRatio}", DebugTextLogChannel.Verbose);
        }

        return damage;
    }

    public bool CanAttackFromPosition(MapMob possibleAttacker, MapMob defender, MapCoordinates attackerPosition)
    {
        return WorldContextInstance.MapHolder.PotentialAttacks(possibleAttacker, attackerPosition).Contains(defender.Position);
    }

    public void RemoveMob(MapMob toRemove)
    {
        if (toRemove == null)
        {
            return;
        }

        DebugTextLog.AddTextToLog($"Removing {toRemove.Name} from the map");
        ActiveMobs.Remove(toRemove);
        Destroy(toRemove.gameObject);
    }

    public MapMob CreateNewUnit(MapCoordinates location, MapMob prefab)
    {
        if (ActiveMobs.Any(mob => mob.Position == location))
        {
            DebugTextLog.AddTextToLog("Tried to create another unit in an occuppied tile.");
            return null;
        }

        MapMob newMob = Instantiate(prefab, MobParent);
        newMob.LoadFromConfiguration(prefab.Configuration);
        newMob.SetPosition(location);
        newMob.ExhaustAllOptions();
        newMob.gameObject.SetActive(true);
        ActiveMobs.Add(newMob);
        return newMob;
    }

    public void CreateNewUnit(MapCoordinates location, MapMob prefab, int teamIndex)
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

    public void LoadFromRealm(Realm toLoad)
    {
        foreach (MobMapData mobData in toLoad.Mobs)
        {
            DebugTextLog.AddTextToLog($"Placing {mobData.MobName} at {mobData.Position.ToString()}), owned by Faction {mobData.Ownership}", DebugTextLogChannel.Verbose);
            CreateNewUnit(mobData);
        }
    }
}
