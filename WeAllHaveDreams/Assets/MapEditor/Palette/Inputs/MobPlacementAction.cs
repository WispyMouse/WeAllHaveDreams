using Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When invoked, places a specified <see cref="MapMob"/> in to the world.
/// </summary>
public class MobPlacementAction : MapEditorInput
{
    /// <summary>
    /// Position to place the Mob.
    /// </summary>
    public MapCoordinates Position;

    /// <summary>
    /// Data of the Mob being placed.
    /// Certainly never null.
    /// </summary>
    public MobMapData Placed;

    /// <summary>
    /// Data of the Mob that was previously on the tile.
    /// May be null if no Mob was present.
    /// </summary>
    public MobMapData Removed;

    /// <summary>
    /// Creates a new MobPlacementAction with the provided Mob at the target Position.
    /// </summary>
    /// <param name="position">Position to place the Mob.</param>
    /// <param name="context">The current WorldContext. Used to determine previous contents.</param>
    /// <param name="toPlace">Mob to place.</param>
    public MobPlacementAction(MapCoordinates position, WorldContext context, MobMapData toPlace)
    {
        Position = position;
        Placed = toPlace;

        MapMob existingMob = context.MobHolder.MobOnPoint(Position);
        if (existingMob != null)
        {
            Removed = existingMob.GetMapData();
        }
    }

    /// <inheritdoc/>
    public override void Invoke(WorldContext worldContextInstance)
    {
        if (Removed != null)
        {
            worldContextInstance.MobHolder.RemoveMob(worldContextInstance.MobHolder.MobOnPoint(Position));
        }

        worldContextInstance.MobHolder.CreateNewUnit(Placed);
    }

    /// <inheritdoc/>
    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.MobHolder.RemoveMob(worldContextInstance.MobHolder.MobOnPoint(Position));

        if (Removed != null)
        {
            worldContextInstance.MobHolder.CreateNewUnit(Removed);
        }
    }
}
