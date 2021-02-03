using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput
{
    public abstract IEnumerator Execute(WorldContext worldContext);

    public virtual bool IsPossible(WorldContext givenContext)
    {
        return true;
    }

    public abstract string LongTitle { get; }
}
