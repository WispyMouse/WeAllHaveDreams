using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeaturePlacementAction : MapEditorInput
{
    public Vector3Int Position;

    public string Removed;
    public string Placed;

    public FeaturePlacementAction(Vector3Int position, string toPlace, WorldContext worldContextInstance)
    {
        Position = position;
        Placed = toPlace;

        MapFeature removedFeature = null;
        if (removedFeature = worldContextInstance.FeatureHolder.FeatureOnPoint(Position))
        {
            Removed = removedFeature.FeatureName;
        }
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.FeatureHolder.SetFeature(Position, FeatureLibrary.GetFeature(Placed));
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.FeatureHolder.SetFeature(Position, FeatureLibrary.GetFeature(Removed));
    }
}
