﻿using Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents painting tiles on to the map.
/// </summary>
public class TilePlacementPalette : PaletteSettings
{
    /// <summary>
    /// A prefab of the tile to place.
    /// </summary>
    public TileConfiguration RepresentedTile;

    /// <summary>
    /// Creates a new TilePlacementPalette.
    /// </summary>
    /// <param name="representedTile">The tile to place.</param>
    public TilePlacementPalette(TileConfiguration representedTile)
    {
        RepresentedTile = representedTile;
    }

    /// <inheritdoc />
    public override Sprite GetButtonSprite()
    {
        return TileLibrary.GetSprite(RepresentedTile.DefaultSprite);
    }

    /// <inheritdoc />
    public override string GetButtonLabel()
    {
        return RepresentedTile.TileName;
    }

    /// <inheritdoc />
    public override MapEditorInput ApplyPalette(WorldContext worldContext, MapCoordinates position)
    {
        return new TileReplacementAction(position, RepresentedTile.TileName, worldContext);
    }
}
