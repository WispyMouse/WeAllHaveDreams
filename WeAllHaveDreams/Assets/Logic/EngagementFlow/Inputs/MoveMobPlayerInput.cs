using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMobPlayerInput : PlayerInput
{
    public MapMob Moving;
    public Vector3Int To;

    public MoveMobPlayerInput(MapMob toMove, Vector3Int to)
    {
        Moving = toMove;
        To = to;
    }

    public override IEnumerator Execute(MapHolder mapHolder, MobHolder mobHolder)
    {
        if (!Moving.CanMove)
        {
            DebugTextLog.AddTextToLog("A unit tried to move, but cannot move.");
            yield break;
        }

        mobHolder.MoveUnit(Moving, To);
        Moving.CanMove = false;
    }
}
