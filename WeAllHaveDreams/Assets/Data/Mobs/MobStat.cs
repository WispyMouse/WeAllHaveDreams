using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MobStat
{
    public string StatName;
    public decimal StatValue;

    public MobStat(string statName, decimal statValue)
    {
        StatName = statName;
        StatValue = statValue;
    }

    public MobStat ApplyAdjustment(StatAdjustment toAdjust)
    {
        decimal newValue = StatValue;

        switch (toAdjust.AmountType)
        {
            case StatAdjustmentAmountType.Flat:
                newValue = StatValue + toAdjust.AmountToChange;
                break;
        }

        return new MobStat(StatName, newValue);
    }
}
