using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipPalette : PaletteSettings
{
    /// <summary>
    /// Gets or sets the global player side ownership setting.
    /// When an OwnershipPalette is selected, this is marked as that button's represented side.
    /// Then when things are painted in other tabs, they are painted with this ownership by default.
    /// </summary>
    public static int? GlobalPlayerSideSetting { get; set; } = null;

    /// <summary>
    /// Gets or sets the value that this Setting paints ownership as.
    /// null represents unowned, and the literal value of null is intentful. null is the default value.
    /// </summary>
    public int? PlayerSide { get; set; } = null;

    public OwnershipPalette()
    {
        PlayerSide = null;
    }

    public OwnershipPalette(int? playerSide)
    {
        PlayerSide = playerSide;
    }

    public override Sprite GetButtonSprite()
    {
        return null;
    }

    public override string GetButtonLabel()
    {
        if (PlayerSide.HasValue)
        {
            return $"Faction {PlayerSide.Value.ToString()}";
        }
        else
        {
            return "Unclaimed";
        }
    }

    public override void PaletteSelected()
    {
        GlobalPlayerSideSetting = PlayerSide;
        DebugTextLog.AddTextToLog($"Setting GlobalPlayerSideSetting to {(PlayerSide.HasValue ? PlayerSide.Value.ToString() : "Unowned")}", DebugTextLogChannel.MapEditorOperations);
        base.PaletteSelected();
    }

    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new OwnershipSetAction(position, worldContext, PlayerSide);
    }
}
