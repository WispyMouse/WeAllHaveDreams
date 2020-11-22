using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput
{
    public abstract void Execute(MapHolder mapHolder, MobHolder mobHolder);
}
