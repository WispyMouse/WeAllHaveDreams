using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipSetAction : MapEditorInput
{
    public Vector3Int Position;

    // TODO: storing the values of each element, and painting them back appropriately instead of taking only first found owned thing
    public int? PreviousValue;
    public int? Value;

    public OwnershipSetAction(Vector3Int position, WorldContext context, int? value)
    {
        Position = position;
        Value = value;

        MapStructure structure;
        MapMob mob;

        if (structure = context.StructureHolder.StructureOnPoint(position))
        {
            PreviousValue = structure.PlayerSideIndex;
        }
        else if (mob = context.MobHolder.MobOnPoint(position))
        {
            PreviousValue = mob.PlayerSideIndex;
        }
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetOwnership(Position, Value);
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetOwnership(Position, PreviousValue);
    }
}
