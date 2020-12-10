using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalBaseStructure : MapStructure
{
    protected override void CompleteCapture(MapMob capturing)
    {
        base.CompleteCapture(capturing);

        DebugTextLog.AddTextToLog("This would result in the game being won for the capturer's team!");
        TurnManager.VictoryIsDeclared (capturing.PlayerSideIndex);
    }
}
