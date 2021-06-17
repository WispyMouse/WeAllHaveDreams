using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalCaptureAbility : StructureConfigurationAbility
{
    public override void OnCapture(MapStructure structure)
    {
        DebugTextLog.AddTextToLog("This would result in the game being won for the capturer's team!", DebugTextLogChannel.Gameplay);
        TurnManager.VictoryIsDeclared(structure.MyPlayerSide);
    }
}
