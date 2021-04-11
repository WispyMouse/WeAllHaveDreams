using Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A palette that represents placings a Mob.
/// </summary>
public class MobPalette : PaletteSettings
{
    /// <summary>
    /// Configuration data for the Mob that this button represents.
    /// </summary>
    public MobConfiguration RepresentedConfiguration;

    public MobPalette(MobConfiguration representedConfiguration)
    {
        RepresentedConfiguration = representedConfiguration;
    }

    /// <inheritdoc />
    public override Sprite GetButtonSprite()
    {
        return MobLibrary.GetMobSprite(RepresentedConfiguration.Appearance, OwnershipPalette.GlobalPlayerSideSetting.HasValue ? OwnershipPalette.GlobalPlayerSideSetting.Value : 0);
    }

    /// <inheritdoc />
    public override string GetButtonLabel()
    {
        return RepresentedConfiguration.Name;
    }

    /// <inheritdoc />
    public override MapEditorInput ApplyPalette(WorldContext worldContext, MapCoordinates position)
    {
        return new MobPlacementAction(position, worldContext, new MobMapData() { MobName = RepresentedConfiguration.Name, Ownership = OwnershipPalette.GlobalPlayerSideSetting.HasValue ? OwnershipPalette.GlobalPlayerSideSetting.Value : 0, Position = position });
    }
}
