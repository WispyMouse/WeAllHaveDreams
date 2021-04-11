using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes a Palette, which can be selected and then applied to the map through <see cref="MapEditorInput.Invoke(WorldContext)"/>.
/// In some cases, there's many PaletteSettings instances for one 'domain'. For example, each Mob type should have its own <see cref="MobPalette"/>.
/// In some cases, there's global configurable PaletteSettings. For example, <see cref="ClearTilePalette"/>.
/// </summary>
public abstract class PaletteSettings
{
    /// <summary>
    /// Gets a Sprite that represents this PaletteSetting, to be displayed in <see cref="PaletteButton"/>.
    /// Can be null if there are no graphics that are appropriate.
    /// </summary>
    /// <returns>A Sprite to use on the Button for this.</returns>
    public virtual Sprite GetButtonSprite()
    {
        return null;
    }

    /// <summary>
    /// Gets a text that represents this PaletteSetting, to be displayed in <see cref="PaletteButton"/>.
    /// Should not be empty or null, as that can make it hard to identify things if there's no sprite for it.
    /// </summary>
    /// <returns>Text to use on the Button for this.</returns>
    public virtual string GetButtonLabel()
    {
        return GetType().Name;
    }

    /// <summary>
    /// Applies actions when a Palette is selected.
    /// Can be overridden to apply extra effects.
    /// </summary>
    public virtual void PaletteSelected()
    {
        DebugTextLog.AddTextToLog($"Selected '{GetButtonLabel()}'", DebugTextLogChannel.MapEditorOperations);
    }

    /// <summary>
    /// Gets the MapEditorInput that represents applying this palette.
    /// Does not actually take action; you'll need to <see cref="MapEditorInput.Invoke(WorldContext)"/> the MapEditorInput.
    /// This should be context dependent in what it returns, storing things like current global settings into the MapEditorInput.
    /// </summary>
    /// <param name="context">The current WorldContext.</param>
    /// <param name="position">The position to apply the Palette.</param>
    /// <returns>An Invoke-able MapEditorInput that applies this Palette.</returns>
    public abstract MapEditorInput ApplyPalette(WorldContext context, MapCoordinates position);
}
