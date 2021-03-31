using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAction : MapEditorInput
{
    public Vector3Int Position;

    string PreviousTile;
    StructureMapData PreviousStructureData;

    public ClearAction(Vector3Int position, WorldContext context)
    {
        Position = position;

        PreviousTile = context.MapHolder.GetGameplayTile(position)?.TileName;

        MapStructure structure = context.StructureHolder.StructureOnPoint(position);
        if (structure != null)
        {
            PreviousStructureData = structure.GetMapData();
        }
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetTile(Position, null);
        worldContextInstance.StructureHolder.ClearStructure(Position);
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetTile(Position, TileLibrary.GetTile(PreviousTile));

        if (PreviousStructureData != null)
        {
            worldContextInstance.StructureHolder.SetStructure(PreviousStructureData);
        }
    }
}
