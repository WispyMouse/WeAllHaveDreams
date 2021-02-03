using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTileFeature : MapFeature
{
    public override bool HasStartOfTurnEffects => true;
    public decimal HealTileHealingAmount => 2M;

    public override void StartOfTurnEffects(MapMob mobOnTile)
    {
        DebugTextLog.AddTextToLog($"Heal Tile healing {mobOnTile.Name} for {HealTileHealingAmount}");
        mobOnTile.HitPoints = System.Math.Min(mobOnTile.MaxHitPoints, mobOnTile.HitPoints + HealTileHealingAmount);
    }
}
