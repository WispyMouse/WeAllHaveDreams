using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAdjustment
{
    public string StatToChange;
    public decimal AmountToChange;
    public StatAdjustmentAmountType AmountType;

    public StatAdjustment(string statToChange, decimal amountTochange, StatAdjustmentAmountType amountType)
    {
        StatToChange = statToChange;
        AmountToChange = amountTochange;
        AmountType = amountType;
    }
}
