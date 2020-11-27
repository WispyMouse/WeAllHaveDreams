using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTurnPlan
{
    public PlayerInput DeterminedInput { get; set; }
    public int Score { get; set; }

    public UnitTurnPlan(PlayerInput fromInput, int determinedScore)
    {
        DeterminedInput = fromInput;
        Score = determinedScore;
    }
}
