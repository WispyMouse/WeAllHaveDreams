using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents painting faction ownership on to tiles.
/// When <see cref="PaletteSelected"/> is called, sets a global default variable for ownership.
/// </summary>
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

    /// <summary>
    /// Creates an OwnershipPalette, with the PlayerSide set to null.
    /// </summary>
    public OwnershipPalette()
    {
        PlayerSide = null;
    }

    /// <summary>
    /// Creates an OwnershipPalette, with the provided PlayerSide.
    /// </summary>
    /// <param name="playerSide">The faction that this Palette should paint as.</param>
    public OwnershipPalette(int? playerSide)
    {
        PlayerSide = playerSide;
    }

    /// <inheritdoc />
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

    /// <summary>
    /// When this Palette is selected, it sets a global static variable that indicates what Ownership to paint things.
    /// </summary>
    public override void PaletteSelected()
    {
        GlobalPlayerSideSetting = PlayerSide;
        DebugTextLog.AddTextToLog($"Setting GlobalPlayerSideSetting to {(PlayerSide.HasValue ? PlayerSide.Value.ToString() : "Unowned")}", DebugTextLogChannel.MapEditorOperations);
        base.PaletteSelected();
    }

    /// <inheritdoc />
    public override MapEditorInput ApplyPalette(WorldContext worldContext, MapCoordinates position)
    {
        return new OwnershipSetAction(position, worldContext, PlayerSide);
    }
}
