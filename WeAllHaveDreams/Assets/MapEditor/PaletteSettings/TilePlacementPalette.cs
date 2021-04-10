using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacementPalette : PaletteSettings
{
    public GameplayTile RepresentedTile;

    public TilePlacementPalette(GameplayTile representedTile)
    {
        RepresentedTile = representedTile;
    }

    public override Sprite GetButtonSprite()
    {
        return RepresentedTile.sprite;
    }

    public override string GetButtonLabel()
    {
        return RepresentedTile.TileName;
    }

    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new TileReplacementAction(position, RepresentedTile.TileName, worldContext);
    }
}
