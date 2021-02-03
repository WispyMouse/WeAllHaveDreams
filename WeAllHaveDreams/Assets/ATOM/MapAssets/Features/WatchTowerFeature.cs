using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchTowerFeature : MapFeature
{
    IEnumerable<StatAdjustment> FlatStatAdjustments => new List<StatAdjustment>() { new StatAdjustment(nameof(MapMob.SightRange), 3, StatAdjustmentAmountType.Flat)  };

    public override IEnumerable<string> Tags => new List<string>() { "Buildable" };

    public override IEnumerable<StatAdjustment> StatAdjustmentsForMob(MapMob mobOnTile)
    {
        return FlatStatAdjustments;
    }
}
