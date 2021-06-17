using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When invoked, sets all objects on the Position to be Owned by the specified faction.
/// </summary>
public class OwnershipSetAction : MapEditorInput
{
    /// <summary>
    /// Position to paint.
    /// </summary>
    public MapCoordinates Position;

    /// <summary>
    /// Ownership value to apply.
    /// This can be null, which would imply that we're removing Ownership from the tile.
    /// Not everything can be painted to null.
    /// </summary>
    public int? Value;

    /// <summary>
    /// Previous Ownership value of things in the tile.
    /// // TODO: storing the values of each element, and painting them back appropriately instead of taking only first found owned thing
    /// </summary>
    public int? PreviousValue;

    /// <summary>
    /// Creates a new OwnershipSetAction.
    /// </summary>
    /// <param name="position">Position to apply Ownership to.</param>
    /// <param name="context">The current WorldContext. Used to determine previous contents.</param>
    /// <param name="value">Value to paint to.</param>
    public OwnershipSetAction(MapCoordinates position, WorldContext context, int? value)
    {
        Position = position;
        Value = value;

        MapStructure structure;
        MapMob mob;

        if (structure = context.StructureHolder.StructureOnPoint(position))
        {
            PreviousValue = structure.MyPlayerSide?.PlayerSideIndex;
        }
        else if (mob = context.MobHolder.MobOnPoint(position))
        {
            PreviousValue = mob.MyPlayerSide?.PlayerSideIndex;
        }
    }

    /// <inheritdoc/>
    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.SetOwnership(Position, Value);
    }

    /// <inheritdoc/>
    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.SetOwnership(Position, PreviousValue);
    }
}
