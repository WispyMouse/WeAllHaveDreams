using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildsAbility : MobConfigurationAbility
{
    public override IEnumerable<PlayerInput> GetPossiblePlayerInputs(MapMob fromMob)
    {
        List<PlayerInput> inputs = new List<PlayerInput>();

        foreach (MapFeature feature in FeatureLibrary.FeaturesWithTags(Arguments))
        {
            inputs.Add(new BuildsFeatureInput(fromMob, feature.name));
        }

        return inputs;
    }
}
