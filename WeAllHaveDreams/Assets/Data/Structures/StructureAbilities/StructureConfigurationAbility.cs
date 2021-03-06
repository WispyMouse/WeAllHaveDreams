﻿using System;
using System.Collections.Generic;

public class StructureConfigurationAbility
{
    public string AbilityName;
    public string[] Arguments;

    public virtual void Load(StructureConfigurationAbility root)
    {
        AbilityName = root.AbilityName;
        Arguments = root.Arguments;
    }

    public virtual IEnumerable<PlayerInput> GetPossiblePlayerInputs(MapStructure structure)
    {
        return new List<PlayerInput>();
    }

    public virtual void OnCapture(MapStructure structure)
    {
        // Do nothing
    }

    public virtual void OnTurnStart(MapStructure structure, MapMob standingOn)
    {
        // Do nothing
    }
}