using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMovementPhase : InputGameplayPhase
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public InputResolutionPhase InputResolutionPhaseInstance;
    public NeutralPhase NeutralPhaseInstance;
    public UnitAttackPhase UnitAttackPhaseInstance;
    public UseAbilityPhase UseAbilityPhaseInstance;

    MapMob selectedUnit { get; set; }

    // HACK: Currently only enabling one ability per mob
    MobConfigurationAbility firstValidAbility { get; set; }

    public UnitMovementPhase UnitSelected(MapMob unit)
    {
        selectedUnit = unit;
        firstValidAbility = null;
        return this;
    }

    public override IEnumerator EnterPhase()
    {
        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            yield break;
        }

        WorldContextInstance.MapMetaHolder.ShowUnitMovementRange(selectedUnit);

        if (selectedUnit.CanAttack)
        {
            WorldContextInstance.MapMetaHolder.ShowUnitAttackRangePastMovementRange(selectedUnit);
            DebugTextLog.AddTextToLog("Press 'A' to enter Attack Only mode", DebugTextLogChannel.DebugOperationInputInstructions);
        }

        MapStructure onStructure;
        if ((onStructure = WorldContextInstance.StructureHolder.StructureOnPoint(selectedUnit.Position)) != null && onStructure.IsNotOwnedByMyTeam(selectedUnit.PlayerSideIndex))
        {
            DebugTextLog.AddTextToLog("Press 'C' to capture this structure", DebugTextLogChannel.DebugOperationInputInstructions);
        }

        foreach (MobConfigurationAbility curAbility in selectedUnit.Abilities)
        {
            IEnumerable<PlayerInput> possibleActions = curAbility.GetPossiblePlayerInputs(selectedUnit);

            if (possibleActions.Any())
            {
                DebugTextLog.AddTextToLog($"Press 'X' to enter {curAbility.AbilityName} mode", DebugTextLogChannel.DebugOperationInputInstructions);
                firstValidAbility = curAbility;
                // HACK: Give units only one ability for now, so we don't need to sort out what button activates them
                break;
            }
        }

        DebugTextLog.AddTextToLog("Press 'Z' to end this mob's move.", DebugTextLogChannel.DebugOperationInputInstructions);
    }

    public override void EndPhase()
    {
        WorldContextInstance.MapMetaHolder.ClearMetas();
    }

    public override void UpdateAfterInput()
    {
        WorldContextInstance.MapMetaHolder.ClearMetas();

        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            return;
        }

        WorldContextInstance.MapMetaHolder.ShowUnitMovementRange(selectedUnit);

        if (selectedUnit.CanAttack)
        {
            WorldContextInstance.MapMetaHolder.ShowUnitAttackRangePastMovementRange(selectedUnit);
        }
    }

    public override bool TryHandleTileClicked(MapCoordinates position, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        // If we can't move, then don't do anything
        if (!selectedUnit.CanMove)
        {
            return false;
        }

        // If the tile isn't in our move range, then don't do anything
        if (!WorldContextInstance.MapMetaHolder.TileIsInActiveMovementRange(position))
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

        // Can we attack them from where we're standing?
        IEnumerable<MapCoordinates> standingAttacks = WorldContextInstance.MapHolder.PotentialAttacks(selectedUnit, mob.Position);
        if (standingAttacks.Contains(mob.Position))
        {
            nextPhase = InputResolutionPhaseInstance.ResolveThis(new AttackWithMobInput(selectedUnit, mob), this);
            return true;
        }

        // What tile can we attack this unit from?
        // We want to pick the closest one to the target out of our possibilities, that is still in this units movement range
        IEnumerable<MapCoordinates> attackingRanges = WorldContextInstance.MapHolder.CanHitFrom(selectedUnit, mob.Position);
        IEnumerable<MapCoordinates> possibleMoves = WorldContextInstance.MapHolder.PotentialMoves(selectedUnit);

        IEnumerable<MapCoordinates> overlap = attackingRanges.Intersect(possibleMoves);

        if (!overlap.Any())
        {
            DebugTextLog.AddTextToLog("That unit is out of this units attack range.");
            return false;
        }

        MapCoordinates closestSpot = overlap.OrderBy(position => Mathf.Abs(position.X - mob.Position.X) + Mathf.Abs(position.Y - mob.Position.Y)
                                                             + Mathf.Abs(position.X - selectedUnit.Position.X) + Mathf.Abs(position.Y - selectedUnit.Position.Y))
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

        if (Input.GetKeyDown(KeyCode.C) && selectedUnit.CanCapture)
        {
            MapStructure onStructure;
            if ((onStructure = WorldContextInstance.StructureHolder.StructureOnPoint(selectedUnit.Position)) != null && onStructure.IsNotOwnedByMyTeam(selectedUnit.PlayerSideIndex))
            {
                nextPhase = InputResolutionPhaseInstance.ResolveThis(new MobCapturesStructurePlayerInput(selectedUnit, onStructure), NeutralPhaseInstance);
                return true;
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && firstValidAbility != null)
        {
            nextPhase = UseAbilityPhaseInstance.AbilitySelected(selectedUnit, firstValidAbility);
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            nextPhase = InputResolutionPhaseInstance.ResolveThis(new DoesNothingPlayerInput(selectedUnit), NeutralPhaseInstance);
            return true;
        }

        return false;
    }
}
