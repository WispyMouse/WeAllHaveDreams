using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTilePalette : PaletteSettings
{
    public ClearTilePalette()
    {
    }

    public override Sprite GetButtonSprite()
    {
        return null;
    }

    public override string GetButtonLabel()
    {
        return "Clear";
    }

    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new ClearAction(position, worldContext);
    }
}
