using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When invoked, places a <see cref="MapStructure"/> at the provided Position.
/// </summary>
public class StructurePlacementAction : MapEditorInput
{
    /// <summary>
    /// Position to place the Structure.
    /// </summary>
    public Vector3Int Position;

    /// <summary>
    /// Data of the Structure to place.
    /// Certainly never null.
    /// </summary>
    public StructureMapData Placed;

    /// <summary>
    /// Data of the Structure that was removed.
    /// May be null if there was no Structure present.
    /// </summary>
    public StructureMapData Removed;

    /// <summary>
    /// Creates a new StructurePlacementAction.
    /// </summary>
    /// <param name="position">Position to place the Structure.</param>
    /// <param name="toPlace">Data of the Structure to place.</param>
    /// <param name="worldContextInstance">The current WorldContext. Used to determine previous contents.</param>
    public StructurePlacementAction(Vector3Int position, StructureMapData toPlace, WorldContext worldContextInstance)
    {
        Position = position;
        Placed = toPlace;

        MapStructure removedStructure = null;
        if (removedStructure = worldContextInstance.StructureHolder.StructureOnPoint(Position))
        {
            Removed = removedStructure.GetMapData();
        }
    }

    /// <inheritdoc />
    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.StructureHolder.SetStructure(Placed);
    }

    /// <inheritdoc />
    public override void Undo(WorldContext worldContextInstance)
    {
        if (Removed != null)
        {
            worldContextInstance.StructureHolder.SetStructure(Removed);
        }
        else
        {
            worldContextInstance.StructureHolder.RemoveStructure(Position);
        }
    }
}
