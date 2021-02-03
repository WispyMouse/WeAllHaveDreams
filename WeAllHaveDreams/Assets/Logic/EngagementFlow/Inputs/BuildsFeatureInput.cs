using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildsFeatureInput : PlayerInput
{
    public MapMob Building;
    public string FeatureToBuild;
    public int Cost;

    public BuildsFeatureInput(MapMob building, string featureToBuild)
    {
        Building = building;

        FeatureToBuild = featureToBuild;
        MapFeature featurePrefab = FeatureLibrary.LookupFeaturePrefab(featureToBuild);
        Cost = featurePrefab.Cost;
    }

    public override string LongTitle => $"Build {FeatureToBuild} for {Cost}";

    public override IEnumerator Execute(WorldContext worldContext)
    {
        throw new System.NotImplementedException();
    }
}
