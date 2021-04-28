using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A palette that represents placing a Feature.
/// </summary>
public class FeaturePlacementPalette : PaletteSettings
{
    /// <summary>
    /// A prefab of what Feature this Palette is representing.
    /// </summary>
    public MapFeature RepresentedFeature;

    /// <summary>
    /// Creates a new FeaturePlacementPalette.
    /// </summary>
    /// <param name="representedFeature">A prefab of what Feature this Palette should apply.</param>
    public FeaturePlacementPalette(MapFeature representedFeature)
    {
        RepresentedFeature = representedFeature;
    }

    /// <inheritdoc />
    public override Sprite GetButtonSprite()
    {
        return RepresentedFeature.Renderer.sprite;
    }

    /// <inheritdoc />
    public override string GetButtonLabel()
    {
        return RepresentedFeature.FeatureName;
    }

    /// <inheritdoc />
    public override MapEditorInput ApplyPalette(WorldContext worldContext, MapCoordinates position)
    {
        return new FeaturePlacementAction(position, RepresentedFeature.FeatureName, worldContext);
    }
}
