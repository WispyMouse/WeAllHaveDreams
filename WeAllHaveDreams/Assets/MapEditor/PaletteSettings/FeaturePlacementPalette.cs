using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeaturePlacementPalette : PaletteSettings
{
    public MapFeature RepresentedFeature;

    public FeaturePlacementPalette(MapFeature representedFeature)
    {
        RepresentedFeature = representedFeature;
    }

    public override Sprite GetButtonSprite()
    {
        return RepresentedFeature.Renderer.sprite;
    }

    public override string GetButtonLabel()
    {
        return RepresentedFeature.FeatureName;
    }

    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new FeaturePlacementAction(position, RepresentedFeature.FeatureName, worldContext);
    }
}
