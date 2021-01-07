using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput
{
    public abstract IEnumerator Execute(MapHolder mapHolder, MobHolder mobHolder);

    public virtual bool IsPossible()
    {
        return true;
    }

    public abstract string LongTitle { get; }
}
