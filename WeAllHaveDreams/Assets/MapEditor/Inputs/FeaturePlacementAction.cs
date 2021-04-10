using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When invoked, places a specified <see cref="MapFeature"/> in to the world.
/// </summary>
public class FeaturePlacementAction : MapEditorInput
{
    /// <summary>
    /// Position to place the Feature.
    /// </summary>
    public Vector3Int Position;

    /// <summary>
    /// The data of the Feature to be placed.
    /// Certainly never null.
    /// </summary>
    public FeatureMapData Placed;

    /// <summary>
    /// The data of the Feature that was removed.
    /// May be null if no tile was previously there.
    /// </summary>
    public FeatureMapData Removed;

    /// <summary>
    /// Creates a new FeaturePlacementAction, to place a specified Feature on a Position.
    /// </summary>
    /// <param name="position">Position to place the Feature.</param>
    /// <param name="toPlace">Name of the Feature to place.</param>
    /// <param name="worldContextInstance">Current world context. The current WorldContext. Used to determine previous contents.</param>
    public FeaturePlacementAction(Vector3Int position, string toPlace, WorldContext worldContextInstance)
    {
        Position = position;
        Placed = new FeatureMapData() { FeatureName = toPlace, Position = Position };

        MapFeature removedFeature = null;
        if (removedFeature = worldContextInstance.FeatureHolder.FeatureOnPoint(Position))
        {
            Removed = removedFeature.GetMapData();
        }
    }

    /// <inheritdoc/>
    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.FeatureHolder.SetFeature(Placed);
    }

    /// <inheritdoc/>
    public override void Undo(WorldContext worldContextInstance)
    {
        if (Removed != null)
        {
            worldContextInstance.FeatureHolder.SetFeature(Removed);
        }
        else
        {
            worldContextInstance.FeatureHolder.SetFeature(Placed.Position, null);
        }
    }
}
