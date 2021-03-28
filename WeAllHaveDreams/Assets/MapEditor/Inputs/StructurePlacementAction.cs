using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePlacementAction : MapEditorInput
{
    public Vector3Int Position;

    public string Removed { get; set; }
    public string Added { get; set; }

    public StructurePlacementAction(Vector3Int position, string toPlace, WorldContext worldContextInstance)
    {
        Position = position;
        Added = toPlace;

        MapStructure removedStructure = null;
        if (removedStructure = worldContextInstance.StructureHolder.StructureOnPoint(Position))
        {
            Removed = removedStructure.StructureName;
        }
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.StructureHolder.SetStructure(Position, StructureLibrary.GetStructure(Added));
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.StructureHolder.SetStructure(Position, StructureLibrary.GetStructure(Removed));
    }
}
