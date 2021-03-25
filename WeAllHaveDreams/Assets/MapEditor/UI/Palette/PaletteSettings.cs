using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes what happens when a Palette is applied to the map.
/// </summary>
public abstract class PaletteSettings
{
    public virtual Sprite GetButtonSprite()
    {
        return null;
    }

    public virtual string GetButtonLabel()
    {
        return string.Empty;
    }

    /// <summary>
    /// Gets the MapEditorInput that represents applying this palette.
    /// Does not actually take action; you'll need to invoke the MapEditorInput.
    /// </summary>
    public abstract MapEditorInput ApplyPalette(WorldContext context, Vector3Int position);
}
