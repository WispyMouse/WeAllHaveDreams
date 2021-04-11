using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildsFeatureInput : PlayerInput
{
    public MapMob Builder;
    public string FeatureToBuild;

    public int Cost { get; set; }
    public MapFeature FeaturePrefab { get; set; }

    public BuildsFeatureInput(MapMob builder, string featureToBuild)
    {
        Builder = builder;

        FeatureToBuild = featureToBuild;
        FeaturePrefab = FeatureLibrary.LookupFeaturePrefab(featureToBuild);
        Cost = FeaturePrefab.Cost;
    }

    public override string LongTitle => $"Build {FeatureToBuild} for {Cost}";

    public override bool IsPossible(WorldContext givenContext)
    {
        return
            TurnManager.CurrentPlayer.TotalResources >= Cost &&
            givenContext.StructureHolder.StructureOnPoint(Builder.Position) == null &&
            givenContext.FeatureHolder.FeatureOnPoint(Builder.Position) == null;
    }

    public override IEnumerator Execute(WorldContext worldContext, GameplayAnimationHolder animationHolder)
    {
        DebugTextLog.AddTextToLog($"Building {FeatureToBuild} at {Builder.Position.ToString()}");
        TurnManager.CurrentPlayer.TotalResources -= Cost;
        worldContext.FeatureHolder.SetFeature(Builder.Position, FeatureLibrary.GetFeature(FeatureToBuild));
        yield return TurnManager.ResolveEffects();
        yield break;
    }
}
