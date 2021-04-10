using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents placing a structure on to the map.
/// </summary>
public class StructurePlacementPalette : PaletteSettings
{
    /// <summary>
    /// A prefab of what Structure to place.
    /// </summary>
    public MapStructure RepresentedStructure;

    /// <summary>
    /// Creates a new StructurePlacementPalette.
    /// </summary>
    /// <param name="representedStructure">A prefab of what Structure to place.</param>
    public StructurePlacementPalette(MapStructure representedStructure)
    {
        RepresentedStructure = representedStructure;
    }

    /// <inheritdoc />
    public override Sprite GetButtonSprite()
    {
        return RepresentedStructure.Renderer.sprite;
    }

    /// <inheritdoc />
    public override string GetButtonLabel()
    {
        return RepresentedStructure.StructureName;
    }

    /// <inheritdoc />
    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new StructurePlacementAction(position, new StructureMapData() { StructureName = RepresentedStructure.StructureName, Ownership = OwnershipPalette.GlobalPlayerSideSetting, Position = position }, worldContext);
    }
}
