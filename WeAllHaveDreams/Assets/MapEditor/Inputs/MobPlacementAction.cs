using Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobPlacementAction : MapEditorInput
{
    public Vector3Int Position;

    public MobConfiguration Placed;
    public MobMapData Removed;
    public int? RemovedFaction;

    public MobPlacementAction(Vector3Int position, WorldContext context, MobConfiguration toPlace)
    {
        Position = position;
        Placed = toPlace;

        MapMob existingMob = context.MobHolder.MobOnPoint(Position);
        if (existingMob != null)
        {
            Removed = existingMob.GetMapData();
            RemovedFaction = existingMob.PlayerSideIndex;
        }
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        if (Removed != null)
        {
            worldContextInstance.MobHolder.RemoveMob(worldContextInstance.MobHolder.MobOnPoint(Position));
        }

        worldContextInstance.MobHolder.CreateNewUnit(Position, MobLibrary.GetMob(Placed.Name), OwnershipPalette.GlobalPlayerSideSetting.HasValue ? OwnershipPalette.GlobalPlayerSideSetting.Value : 0);
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.MobHolder.RemoveMob(worldContextInstance.MobHolder.MobOnPoint(Position));

        if (Removed != null)
        {
            worldContextInstance.MobHolder.CreateNewUnit(Removed);
        }
    }
}
