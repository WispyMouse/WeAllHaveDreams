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
}
