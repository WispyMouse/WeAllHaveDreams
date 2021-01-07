using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoesNothingPlayerInput : PlayerInput
{
    MapMob notActing;

    public DoesNothingPlayerInput(MapMob doingNothing)
    {
        notActing = doingNothing;
    }

    public override string LongTitle => $"Do nothing and exhaust all options";

    public override IEnumerator Execute(MapHolder mapHolder, MobHolder mobHolder)
    {
        notActing.ExhaustAllOptions();
        yield break;
    }
}
