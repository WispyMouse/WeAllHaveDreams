using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldContext : MonoBehaviour
{
    public MapHolder MapHolder;
    public MobHolder MobHolder;
    public StructureHolder StructureHolder;
    public FeatureHolder FeatureHolder;
    public FogHolder FogHolder;

    public void ClearEverything()
    {
        MobHolder.ClearAllMobs();
        StructureHolder.ClearAllStructures();
        FeatureHolder.ClearAllFeatures();
        FogHolder.ClearAllTiles();
    }
}
