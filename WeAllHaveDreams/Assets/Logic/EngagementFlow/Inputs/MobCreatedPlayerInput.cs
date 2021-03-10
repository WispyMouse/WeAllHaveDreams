using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCreatedPlayerInput : PlayerInput
{
    public MapStructure AtStructure;
    public MapMob MapMobPf;
    public int ResourceCost;

    public MobCreatedPlayerInput(MapStructure atStructure, MapMob mapMobPf, int resourceCost)
    {
        AtStructure = atStructure;
        MapMobPf = mapMobPf;
        ResourceCost = resourceCost;
    }

    public override string LongTitle => $"Create {MapMobPf.name} for {ResourceCost} resources";

    public override IEnumerator Execute(WorldContext worldContext, GameplayAnimationHolder animationHolder)
    {
        if (TurnManager.CurrentPlayer.TotalResources >= ResourceCost)
        {
            worldContext.MobHolder.CreateNewUnit(AtStructure.Position, MapMobPf, AtStructure.PlayerSideIndex);
            TurnManager.CurrentPlayer.TotalResources -= ResourceCost;
        }
        else
        {
            DebugTextLog.AddTextToLog($"Not enough resources! Need {ResourceCost}, have {TurnManager.CurrentPlayer.TotalResources}.");
        }

        yield return TurnManager.ResolveEffects();
    }
}
