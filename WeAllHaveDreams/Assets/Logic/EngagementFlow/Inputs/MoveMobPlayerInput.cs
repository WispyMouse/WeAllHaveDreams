﻿using System.Collections;
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

    public override void Execute(MapHolder mapHolder, MobHolder mobHolder)
    {
        mobHolder.MoveUnit(Moving, To);
        Moving.CanMove = false;
        Moving.HideReminder(nameof(Moving.CanMove));
    }
}