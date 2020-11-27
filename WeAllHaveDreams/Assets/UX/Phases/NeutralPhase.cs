﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralPhase : InputGameplayPhase
{
    public UnitMovementPhase UnitMovementPhaseInstance;
    public MobHolder MobHolderInstance;
    public StructureUsagePhase StructureUsagePhaseInstance;

    public override bool TryHandleUnitClicked(MapMob mob, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        // If they're not on our team, there's nothing to do.
        if (mob.PlayerSideIndex != TurnManager.CurrentPlayer.PlayerSideIndex)
        {
            return false;
        }

        nextPhase = UnitMovementPhaseInstance.UnitSelected(mob);
        return true;
    }

    public override bool TryHandleStructureClicked(MapStructure structure, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;

        // If they're not on our team, there's nothing to do.
        if (!structure.UnCaptured && structure.PlayerSideIndex != TurnManager.CurrentPlayer.PlayerSideIndex)
        {
            return false;
        }

        nextPhase = StructureUsagePhaseInstance.StructureSelected(structure);
        return true;
    }
}
