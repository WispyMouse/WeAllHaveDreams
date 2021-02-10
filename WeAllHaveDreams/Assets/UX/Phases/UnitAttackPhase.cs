using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAttackPhase : InputGameplayPhase
{
    MapMob selectedUnit { get; set; }

    public MapMeta MapMetaInstance;
    public MapHolder MapHolderInstance;

    public UnitMovementPhase UnitMovementPhaseInstance;
    public NeutralPhase NeutralPhaseInstance;
    public InputResolutionPhase InputResolutionPhaseInstance;
    public StructureHolder StructureHolderInstance;
    public UseAbilityPhase UseAbilityPhaseInstance;

    // HACK: Currently only enabling one ability per mob
    MobConfigurationAbility firstValidAbility { get; set; }

    public UnitAttackPhase UnitSelected(MapMob unit)
    {
        selectedUnit = unit;
        firstValidAbility = null;
        return this;
    }

    public override IEnumerator EnterPhase()
    {
        // If we can't attack, then don't do anything
        if (!selectedUnit.CanAttack)
        {
            yield break;
        }

        MapMetaInstance.ShowUnitAttackRange(selectedUnit);

        if (selectedUnit.CanMove)
        {
            DebugTextLog.AddTextToLog("Press 'M' to switch to Move mode", DebugTextLogChannel.DebugOperationInputInstructions);
        }

        MapStructure onStructure;
        if ((onStructure = StructureHolderInstance.StructureOnPoint(selectedUnit.Position)) != null && onStructure.IsNotOwnedByMyTeam(selectedUnit.PlayerSideIndex))
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
        MapMetaInstance.ClearMetas();
    }

    public override void UpdateAfterInput()
    {
        MapMetaInstance.ClearMetas();

        // If we can't move, then don't do anything
        if (!selectedUnit.CanAttack)
        {
            return;
        }

        MapMetaInstance.ShowUnitAttackRange(selectedUnit);
    }

    public override bool TryHandleUnitClicked(MapMob mob, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        // If we click on an ally, select them
        if (mob.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex)
        {
            nextPhase = UnitMovementPhaseInstance.UnitSelected(mob);
            return true;
        }

        IEnumerable<Vector3Int> attackingRanges = MapHolderInstance.CanHitFrom(selectedUnit, mob.Position);
        if (!attackingRanges.Contains(selectedUnit.Position))
        {
            DebugTextLog.AddTextToLog("That unit is out of this units attack range.");
            return false;
        }

        nextPhase = InputResolutionPhaseInstance.ResolveThis(new AttackWithMobInput(selectedUnit, mob), UnitMovementPhaseInstance.UnitSelected(selectedUnit));
        return true;
    }

    public override bool TryHandleKeyPress(out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        if (Input.GetKeyDown(KeyCode.M))
        {
            nextPhase = UnitMovementPhaseInstance.UnitSelected(selectedUnit);
            return true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            MapStructure onStructure;
            if ((onStructure = StructureHolderInstance.StructureOnPoint(selectedUnit.Position)) != null && onStructure.IsNotOwnedByMyTeam(selectedUnit.PlayerSideIndex))
            {
                nextPhase = InputResolutionPhaseInstance.ResolveThis(new MobCapturesStructurePlayerInput(selectedUnit, onStructure), UnitMovementPhaseInstance.UnitSelected(selectedUnit));
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
