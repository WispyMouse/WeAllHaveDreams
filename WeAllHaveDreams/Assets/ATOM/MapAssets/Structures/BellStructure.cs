using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellStructure : MapStructure
{
    public List<string> SoldUnits;

    public override IEnumerable<PlayerInput> GetPossiblePlayerInputs(MobHolder mobHolderInstance)
    {
        List<PlayerInput> possibleInputs = new List<PlayerInput>();
        foreach (string soldUnit in SoldUnits)
        {
            MapMob matchingUnit = MobLibrary.GetMob(soldUnit);
            possibleInputs.Add(new MobCreatedPlayerInput(this, matchingUnit, matchingUnit.ResourceCost));
        }
        return possibleInputs;
    }
}
