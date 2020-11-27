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

    public override IEnumerator Execute(MapHolder mapHolder, MobHolder mobHolder)
    {
        if (TurnManager.CurrentPlayer.TotalResources >= ResourceCost)
        {
            mobHolder.CreateNewUnit(MapMobPf, AtStructure.PlayerSideIndex, AtStructure.Position);
        }
        else
        {
            DebugTextLog.AddTextToLog($"Not enough resources! Need {ResourceCost}, have {TurnManager.CurrentPlayer.TotalResources}.");
        }

        yield return TurnManager.ResolveEffects();
    }
}
