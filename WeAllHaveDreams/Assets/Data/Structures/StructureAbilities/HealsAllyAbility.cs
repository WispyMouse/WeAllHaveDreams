using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsAllyAbility : StructureConfigurationAbility
{
    public override void OnTurnStart(MapStructure structure, MapMob standingOn)
    {
        if (standingOn == null)
        {
            return;
        }

        if (structure.PlayerSideIndex != standingOn.PlayerSideIndex)
        {
            return;
        }

        // No need to heal someone at full health
        if (standingOn.HitPoints == standingOn.MaxHitPoints)
        {
            return;
        }

        decimal healAmount = decimal.Parse(Arguments[0]);

        DebugTextLog.AddTextToLog($"Structure healing {standingOn.Name} for {healAmount} (up to {standingOn.MaxHitPoints})");
        standingOn.HitPoints = System.Math.Min(standingOn.MaxHitPoints, standingOn.HitPoints + healAmount);
    }
}
