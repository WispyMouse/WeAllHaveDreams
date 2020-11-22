using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralPhase : InputGameplayPhase
{
    public UnitMovementPhase UnitMovementPhaseInstance;

    public override InputGameplayPhase UnitClicked(MapMob mob)
    {
        // If they're not on our team, there's nothing to do.
        if (mob.PlayerSideIndex != TurnManager.CurrentPlayer.PlayerSideIndex)
        {
            return this;
        }

        return UnitMovementPhaseInstance.UnitSelected(mob);
    }
}
