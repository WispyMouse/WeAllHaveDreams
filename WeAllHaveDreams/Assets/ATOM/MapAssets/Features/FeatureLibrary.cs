using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatureLibrary : SingletonBase<FeatureLibrary>
{
    public MapFeature[] Features;
    Dictionary<string, MapFeature> NamesToFeatures { get; set; } = new Dictionary<string, MapFeature>();

    public MapFeature DefaultFeature;

    public static MapFeature GetFeature(string featureName)
    {
        if (Singleton.NamesToFeatures.TryGetValue(featureName, out MapFeature foundFeature))
        {
            return Instantiate(foundFeature);
        }

        MapFeature matchingFeature = Singleton.Features.FirstOrDefault(feature => feature.name == featureName);

        if (matchingFeature == null)
        {
            DebugTextLog.AddTextToLog($"Could not find a Feature in the Library with the name {featureName}. Returning a default.");
            return Instantiate(Singleton.DefaultFeature);
        }

        Singleton.NamesToFeatures.Add(featureName, matchingFeature);
        return Instantiate(matchingFeature);
    }
}
