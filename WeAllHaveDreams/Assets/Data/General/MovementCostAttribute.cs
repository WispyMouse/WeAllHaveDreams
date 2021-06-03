using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCostAttribute : TaggedAttribute
{
    public const int TypicalMovementCost = 1;

    /// <summary>
    /// Indicates what kind of change happens to movement.
    /// </summary>
    public MovementModificationEnum MovementModification;

    /// <summary>
    /// An argument for how the change is affected.
    /// Not always relevant.
    /// </summary>
    public decimal Value;
}
