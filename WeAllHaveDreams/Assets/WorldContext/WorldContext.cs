using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldContext : SingletonBase<WorldContext>
{
    public MapHolder MapHolder;
    public MapMetaHolder MapMetaHolder;
    public MobHolder MobHolder;
    public StructureHolder StructureHolder;
    public FeatureHolder FeatureHolder;
    public FogHolder FogHolder;

    public void ClearEverything()
    {
        MapHolder.ClearEverything();
        MobHolder.ClearAllMobs();
        MapMetaHolder.ClearMetas();
        StructureHolder.ClearAllStructures();
        FeatureHolder.ClearAllFeatures();
        FogHolder.ClearAllTiles();
    }

    public static WorldContext GetWorldContext()
    {
        return Singleton;
    }

    public void LoadFromRealm(Realm toLoad)
    {
        MapHolder.LoadFromRealm(toLoad);
        StructureHolder.LoadFromRealm(toLoad);
        MobHolder.LoadFromRealm(toLoad);
        FeatureHolder.LoadFromRealm(toLoad);
    }

    public Realm GenerateRealm()
    {
        Realm newRealm = new Realm();

        List<RealmCoordinate> realmCoordinates = new List<RealmCoordinate>();
        List<StructureMapData> structures = new List<StructureMapData>();
        List<MobMapData> mobs = new List<MobMapData>();
        List<FeatureMapData> features = new List<FeatureMapData>();

        foreach (Vector3Int position in MapHolder.GetAllTiles())
        {
            GameplayTile tile = MapHolder.GetGameplayTile(position);
            realmCoordinates.Add(new RealmCoordinate() { Position = position, Tile = tile.TileName });

            MapStructure structureOnPoint;
            if (structureOnPoint = StructureHolder.StructureOnPoint(position))
            {
                structures.Add(structureOnPoint.GetMapData());
            }

            MapMob mobOnPoint;
            if (mobOnPoint = MobHolder.MobOnPoint(position))
            {
                mobs.Add(mobOnPoint.GetMapData());
            }

            MapFeature featureOnPoint;
            if (featureOnPoint = FeatureHolder.FeatureOnPoint(position))
            {
                features.Add(featureOnPoint.GetMapData());
            }
        }

        newRealm.RealmCoordinates = realmCoordinates;
        newRealm.Structures = structures;
        newRealm.Mobs = mobs;
        newRealm.Features = features;

        return newRealm;
    }


    public void SetOwnership(Vector3Int position, int? value)
    {
        MapStructure structure;
        if (structure = StructureHolder.StructureOnPoint(position))
        {
            structure.SetOwnership(value);
        }

        MapMob mob;
        if (mob = MobHolder.MobOnPoint(position))
        {
            mob.SetOwnership(value.Value);
        }
    }
}
