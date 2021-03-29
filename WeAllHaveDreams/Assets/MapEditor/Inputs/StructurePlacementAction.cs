using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePlacementAction : MapEditorInput
{
    public Vector3Int Position;

    public StructureMapData Removed;
    public StructureMapData Placed;

    public StructurePlacementAction(Vector3Int position, string toPlace, WorldContext worldContextInstance)
    {
        Position = position;
        Placed = new StructureMapData() { Position = position, StructureName = toPlace, Ownership = OwnershipPalette.GlobalPlayerSideSetting };

        MapStructure removedStructure = null;
        if (removedStructure = worldContextInstance.StructureHolder.StructureOnPoint(Position))
        {
            Removed = removedStructure.GetMapData();
        }
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.StructureHolder.SetStructure(Placed);
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.StructureHolder.SetStructure(Removed);
    }
}
