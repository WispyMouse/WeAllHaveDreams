using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTurnPlan
{
    public PlayerInput DeterminedInput { get; set; }
    public decimal Score { get; set; }

    public UnitTurnPlan(PlayerInput fromInput, decimal determinedScore)
    {
        DeterminedInput = fromInput;
        Score = determinedScore;
    }
}
