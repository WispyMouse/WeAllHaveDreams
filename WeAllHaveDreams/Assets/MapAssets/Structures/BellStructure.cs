using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellStructure : MapStructure
{
    public MapMob MapMobPf;

    public override void DoLazyBuildingThing(MobHolder mobHolderInstance)
    {
        mobHolderInstance.CreateNewUnit(MapMobPf, PlayerSideIndex, Position);
    }
}
