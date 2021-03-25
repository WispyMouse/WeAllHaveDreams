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

    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new TileReplacementAction(position, worldContext.MapHolder.GetGameplayTile(position)?.TileName, RepresentedTile.TileName);
    }
}
