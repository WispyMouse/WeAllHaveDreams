using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellStructure : MapStructure
{
    public int CostOfUnit = 100;
    public MapMob MapMobPf;

    public override PlayerInput DoLazyBuildingThing(MobHolder mobHolderInstance)
    {
        DebugTextLog.AddTextToLog($"Attempting to use {CostOfUnit} resources to make <mobname>");
        return new MobCreatedPlayerInput(this, MapMobPf, CostOfUnit);
    }
}
