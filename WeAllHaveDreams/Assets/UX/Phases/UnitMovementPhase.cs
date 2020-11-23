using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMovementPhase : InputGameplayPhase
{
    public InputResolutionPhase InputResolutionPhaseInstance;
    public NeutralPhase NeutralPhaseInstance;
    public UnitAttackPhase UnitAttackPhaseInstance;

    public MapMeta MapMetaInstance;
    public MapHolder MapHolderInstance;

    MapMob selectedUnit { get; set; }

    public UnitMovementPhase UnitSelected(MapMob unit)
    {
        selectedUnit = unit;
        return this;
    }

    public override IEnumerator EnterPhase()
    {
        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            yield break;
        }

        MapMetaInstance.ShowUnitMovementRange(selectedUnit);

        if (selectedUnit.CanAttack)
        {
            MapMetaInstance.ShowUnitAttackRangePastMovementRange(selectedUnit);
            DebugTextLog.AddTextToLog("Press 'A' to enter Attack Only mode");
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

    public override bool TryHandleTileClicked(Vector3Int position, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            return false;
        }

        // If the tile isn't in our move range, then don't do anything
        if (!MapMetaInstance.TileIsInActiveMovementRange(position))
        {
            return false;
        }

        nextPhase = InputResolutionPhaseInstance.ResolveThis(new MoveMobPlayerInput(selectedUnit, position), this);
        return true;
    }

    public override bool TryHandleUnitClicked(MapMob mob, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        // If we click on an ally, select them
        if (mob.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex)
        {
            nextPhase = UnitSelected(mob);
            return true;
        }

        // What tile can we attack this unit from?
        // We want to pick the closest one to the target out of our possibilities, that is still in this units movement range
        IEnumerable<Vector3Int> attackingRanges = MapHolderInstance.CanHitFrom(selectedUnit, mob.Position);
        IEnumerable<Vector3Int> possibleMoves = MapHolderInstance.PotentialMoves(selectedUnit);

        IEnumerable<Vector3Int> overlap = attackingRanges.Intersect(possibleMoves);

        if (!overlap.Any())
        {
            DebugTextLog.AddTextToLog("That unit is out of this units attack range.");
            return false;
        }

        Vector3Int closestSpot = overlap.OrderBy(position => Mathf.Abs(position.x - mob.Position.x) + Mathf.Abs(position.y - mob.Position.y)
                                                             + Mathf.Abs(position.x - selectedUnit.Position.x) + Mathf.Abs(position.y - selectedUnit.Position.y))
                                        .First();

        // We're already there! No need to walk
        if (closestSpot == selectedUnit.Position)
        {
            nextPhase = InputResolutionPhaseInstance.ResolveThis(new AttackWithMobInput(selectedUnit, mob, closestSpot), this);
        }
        else
        {
            nextPhase = InputResolutionPhaseInstance.ResolveThis(new AttackWithMobInput(selectedUnit, mob, closestSpot), NeutralPhaseInstance);
        }

        return true;
    }

    public override bool WaitingForInput
    {
        get
        {
            return !NextPhasePending;
        }
    }

    public override bool NextPhasePending
    {
        get
        {
            return !selectedUnit.CanMove;
        }
    }

    public override InputGameplayPhase GetNextPhase()
    {
        if (selectedUnit.CanAttack)
        {
            return UnitAttackPhaseInstance.UnitSelected(selectedUnit);
        }

        return NeutralPhaseInstance;
    }

    public override bool TryHandleKeyPress(out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        if (Input.GetKeyDown(KeyCode.A))
        {
            nextPhase = UnitAttackPhaseInstance.UnitSelected(selectedUnit);
            return true;
        }

        return false;
    }
}
