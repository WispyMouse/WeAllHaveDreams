using Configuration;
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
    public StructureConfiguration RepresentedStructure;

    /// <summary>
    /// Creates a new StructurePlacementPalette.
    /// </summary>
    /// <param name="representedStructure">A prefab of what Structure to place.</param>
    public StructurePlacementPalette(StructureConfiguration representedStructure)
    {
        RepresentedStructure = representedStructure;
    }

    /// <inheritdoc />
    public override Sprite GetButtonSprite()
    {
        return StructureLibrary.GetStructureSprite(RepresentedStructure.Appearance, null);
    }

    /// <inheritdoc />
    public override string GetButtonLabel()
    {
        return RepresentedStructure.Name;
    }

    /// <inheritdoc />
    public override MapEditorInput ApplyPalette(WorldContext worldContext, MapCoordinates position)
    {
        return new StructurePlacementAction(position, new StructureMapData() { StructureName = RepresentedStructure.DevelopmentName, Ownership = OwnershipPalette.GlobalPlayerSideSetting, Position = position }, worldContext);
    }
}
