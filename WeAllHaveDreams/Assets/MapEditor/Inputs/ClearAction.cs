using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAction : MapEditorInput
{
    public Vector3Int Position;

    string PreviousTile;
    StructureMapData PreviousStructureData;
    MobMapData PreviousMobData;
    FeatureMapData PreviousFeatureData;

    public ClearAction(Vector3Int position, WorldContext context)
    {
        Position = position;

        PreviousTile = context.MapHolder.GetGameplayTile(position)?.TileName;

        MapStructure structure = context.StructureHolder.StructureOnPoint(position);
        if (structure != null)
        {
            PreviousStructureData = structure.GetMapData();
        }

        MapMob mob = context.MobHolder.MobOnPoint(position);
        if (mob != null)
        {
            PreviousMobData = mob.GetMapData();
        }

        MapFeature feature = context.FeatureHolder.FeatureOnPoint(position);
        if (feature != null)
        {
            PreviousFeatureData = feature.GetMapData();
        }
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        if (PreviousTile != null)
        {
            worldContextInstance.MapHolder.SetTile(Position, null);
        }

        if (PreviousStructureData != null)
        {
            worldContextInstance.StructureHolder.ClearStructure(Position);
        }

        if (PreviousMobData != null)
        {
            worldContextInstance.MobHolder.RemoveMob(worldContextInstance.MobHolder.MobOnPoint(Position));
        }

        if (PreviousFeatureData != null)
        {
            worldContextInstance.FeatureHolder.SetFeature(Position, null);
        }
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        if (PreviousTile != null)
        {
            worldContextInstance.MapHolder.SetTile(Position, TileLibrary.GetTile(PreviousTile));
        }

        if (PreviousStructureData != null)
        {
            worldContextInstance.StructureHolder.SetStructure(PreviousStructureData);
        }

        if (PreviousMobData != null)
        {
            worldContextInstance.MobHolder.CreateNewUnit(PreviousMobData);
        }

        if (PreviousFeatureData != null)
        {
            worldContextInstance.FeatureHolder.SetFeature(PreviousFeatureData);
        }
    }
}
