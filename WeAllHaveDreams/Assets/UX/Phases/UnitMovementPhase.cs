using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMovementPhase : InputGameplayPhase
{
    public InputResolutionPhase InputResolutionPhaseInstance;
    public NeutralPhase NeutralPhaseInstance;
    public MapMeta MapMetaInstance;
    public MapHolder MapHolderInstance;

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

        if (selectedUnit.CanAttack)
        {
            MapMetaInstance.ShowUnitAttackRangePastMovementRange(selectedUnit);
        }
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

        if (selectedUnit.CanAttack)
        {
            MapMetaInstance.ShowUnitAttackRangePastMovementRange(selectedUnit);
        }
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

        // What tile can we attack this unit from?
        // We want to pick the closest one to the target out of our possibilities, that is still in this units movement range
        IEnumerable<Vector3Int> attackingRanges = MapHolderInstance.CanHitFrom(selectedUnit, mob.Position);
        IEnumerable<Vector3Int> possibleMoves = MapHolderInstance.PotentialMoves(selectedUnit);

        IEnumerable<Vector3Int> overlap = attackingRanges.Intersect(possibleMoves);

        if (!overlap.Any())
        {
            DebugTextLog.AddTextToLog("That unit is out of this units attack range.");
            return this;
        }

        Vector3Int closestSpot = overlap.OrderBy(position => Mathf.Abs(position.x - mob.Position.x) + Mathf.Abs(position.y - mob.Position.y) 
                                                             + Mathf.Abs(position.x - selectedUnit.Position.x) + Mathf.Abs(position.y - selectedUnit.Position.y))
                                        .First();

        // We're already there! No need to walk
        if (closestSpot == selectedUnit.Position)
        {
            return InputResolutionPhaseInstance.ResolveThis(new AttackWithMobInput(selectedUnit, mob, closestSpot), this);
        }
        else
        {
            return InputResolutionPhaseInstance.ResolveThis(new AttackWithMobInput(selectedUnit, mob, closestSpot), NeutralPhaseInstance);
        }
    }
}
