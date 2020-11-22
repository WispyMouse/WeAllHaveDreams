using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementPhase : InputGameplayPhase
{
    public InputResolutionPhase InputResolutionPhaseInstance;
    public MapMeta MapMetaInstance;

    MapMob selectedUnit { get; set; }

    public UnitMovementPhase UnitSelected(MapMob unit)
    {
        selectedUnit = unit;
        return this;
    }

    public override void EnterPhase()
    {
        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            return;
        }

        MapMetaInstance.ShowUnitMovementRange(selectedUnit);
    }

    public override void EndPhase()
    {
        MapMetaInstance.ClearMetas();
    }

    public override void UpdateAfterInput()
    {
        MapMetaInstance.ClearMetas();

        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            return;
        }

        MapMetaInstance.ShowUnitMovementRange(selectedUnit);
    }

    public override InputGameplayPhase TileClicked(Vector3Int position)
    {
        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            return this;
        }

        // If the tile isn't in our move range, then don't do anything
        if (!MapMetaInstance.TileIsInActiveMovementRange(position))
        {
            return this;
        }

        return InputResolutionPhaseInstance.ResolveThis(new MoveMobPlayerInput(selectedUnit, position), this);
    }

    public override InputGameplayPhase UnitClicked(MapMob mob)
    {
        // If we click on an ally, select them
        if (mob.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex)
        {
            return UnitSelected(mob);
        }

        return this;
    }
}
