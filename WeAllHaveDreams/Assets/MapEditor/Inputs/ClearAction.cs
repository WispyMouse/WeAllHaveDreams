using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When invoked, this MapEditorInput removes all content from a position.
/// TODO: Set this up to possibly remove only certain layers, rather than always everything.
/// </summary>
public class ClearAction : MapEditorInput
{
    /// <summary>
    /// Coordinate for this Action.
    /// </summary>
    public Vector3Int Position;

    /// <summary>
    /// Name of the <see cref="GameplayTile"/> that was previously at <see cref="Position"/>.
    /// May be null if no Tile was present.
    /// </summary>
    public string PreviousTile;

    /// <summary>
    /// Data for the Structure that was previously at <see cref="Position"/>.
    /// May be null if no Structure was present.
    /// </summary>
    public StructureMapData PreviousStructureData;

    /// <summary>
    /// Data for the Mob that was previously at <see cref="Position"/>.
    /// May be null if no Mob was present.
    /// </summary>
    public MobMapData PreviousMobData;

    /// <summary>
    /// Data for the Feature that was previously at <see cref="Position"/>.
    /// May be null if no Feature was present.
    /// </summary>
    public FeatureMapData PreviousFeatureData;

    /// <summary>
    /// Creates a new ClearAction for the specified position.
    /// </summary>
    /// <param name="position">Position for this to apply to.</param>
    /// <param name="context">The current WorldContext. This is used to store the content that was previously on this position.</param>
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

    /// <inheritdoc/>
    public override void Invoke(WorldContext worldContextInstance)
    {
        if (PreviousTile != null)
        {
            worldContextInstance.MapHolder.SetTile(Position, null);
        }

        if (PreviousStructureData != null)
        {
            worldContextInstance.StructureHolder.RemoveStructure(Position);
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

    /// <inheritdoc/>
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
