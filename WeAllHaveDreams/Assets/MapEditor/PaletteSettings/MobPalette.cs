using Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobPalette : PaletteSettings
{
    public MobConfiguration RepresentedConfiguration;

    public MobPalette(MobConfiguration representedConfiguration)
    {
        RepresentedConfiguration = representedConfiguration;
    }

    public override Sprite GetButtonSprite()
    {
        return MobLibrary.GetMobSprite(RepresentedConfiguration.Appearance, 0);
    }

    public override string GetButtonLabel()
    {
        return RepresentedConfiguration.Name;
    }

    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new MobPlacementAction(position, worldContext, RepresentedConfiguration);
    }
}
