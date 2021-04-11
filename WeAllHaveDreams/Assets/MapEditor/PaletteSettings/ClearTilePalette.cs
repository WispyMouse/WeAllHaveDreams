using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Palette that represents removing the contents of a position.
/// TODO: This should be configurable to only remove specified layers, not everything.
/// </summary>
public class ClearTilePalette : PaletteSettings
{
    /// <inheritdoc />
    public override string GetButtonLabel()
    {
        return "Clear";
    }

    /// <inheritdoc />
    public override MapEditorInput ApplyPalette(WorldContext worldContext, MapCoordinates position)
    {
        return new ClearAction(position, worldContext);
    }
}
