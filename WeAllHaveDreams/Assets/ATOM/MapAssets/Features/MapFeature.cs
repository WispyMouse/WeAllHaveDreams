using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFeature : MapObject
{
    public int Cost { get; set; }
    public string FeatureName;

    public SpriteRenderer Renderer;

    public FeatureMapData GetMapData()
    {
        return new FeatureMapData() { FeatureName = FeatureName, Position = Position };
    }
}
