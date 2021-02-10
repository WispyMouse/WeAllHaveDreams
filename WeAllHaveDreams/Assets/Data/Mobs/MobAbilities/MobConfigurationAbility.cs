using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobConfigurationAbility
{
    public string AbilityName;
    public string[] Arguments;

    public virtual void Load(MobConfigurationAbility root)
    {
        AbilityName = root.AbilityName;
        Arguments = root.Arguments;
    }

    public virtual IEnumerable<PlayerInput> GetPossiblePlayerInputs(MapMob fromMob)
    {
        return new List<PlayerInput>();
    }
}
