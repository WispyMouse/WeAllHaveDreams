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
    public InputResolutionPhase InputResolutionPhaseInstance;

    public UnitAttackPhase UnitSelected(MapMob unit)
    {
        selectedUnit = unit;
        return this;
    }

    public override void EnterPhase()
    {
        // If we can't attack, then don't do anything
        if (!selectedUnit.CanAttack)
        {
            return;
        }

        MapMetaInstance.ShowUnitAttackRange(selectedUnit);

        if (selectedUnit.CanMove)
        {
            DebugTextLog.AddTextToLog("Press 'M' to switch to Move mode");
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
        if (!selectedUnit.CanAttack)
        {
            return;
        }

        MapMetaInstance.ShowUnitAttackRange(selectedUnit);
    }

    public override InputGameplayPhase UnitClicked(MapMob mob)
    {
        // If we click on an ally, select them
        if (mob.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex)
        {
            return UnitMovementPhaseInstance.UnitSelected(mob);
        }

        IEnumerable<Vector3Int> attackingRanges = MapHolderInstance.CanHitFrom(selectedUnit, mob.Position);
        if (!attackingRanges.Contains(selectedUnit.Position))
        {
            DebugTextLog.AddTextToLog("That unit is out of this units attack range.");
            return this;
        }

        return InputResolutionPhaseInstance.ResolveThis(new AttackWithMobInput(selectedUnit, mob), UnitMovementPhaseInstance.UnitSelected(selectedUnit));
    }

    public override bool TryHandleKeyPress(out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        if (Input.GetKeyDown(KeyCode.M))
        {
            nextPhase = UnitMovementPhaseInstance.UnitSelected(selectedUnit);
            return true;
        }

        return false;
    }
}
