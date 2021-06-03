using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DefensiveAttributes : TaggedAttribute
{
    public const int NoDefense = 1;

    /// <summary>
    /// The percentage of damage that you take on this tile.
    /// 1 means you take 100% damage. Essentially no mitigation.
    /// 0 means you would take no damage, regardless of the attack's raw output.
    /// 0.8 means you take 80% of the damage coming in, acting as 20% damage mitigation.
    /// NOTE: JSON requires 0.2, rather than .2, in the text file.
    /// </summary>
    public decimal DefensiveRatio;
}
