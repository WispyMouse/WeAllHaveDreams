using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePlacementPalette : PaletteSettings
{
    public MapStructure RepresentedStructure;

    public StructurePlacementPalette(MapStructure representedStructure)
    {
        RepresentedStructure = representedStructure;
    }

    public override Sprite GetButtonSprite()
    {
        return RepresentedStructure.Renderer.sprite;
    }

    public override string GetButtonLabel()
    {
        return RepresentedStructure.StructureName;
    }

    public override MapEditorInput ApplyPalette(WorldContext worldContext, Vector3Int position)
    {
        return new StructurePlacementAction(position, new StructureMapData() { StructureName = RepresentedStructure.StructureName, Ownership = OwnershipPalette.GlobalPlayerSideSetting, Position = position }, worldContext);
    }
}
