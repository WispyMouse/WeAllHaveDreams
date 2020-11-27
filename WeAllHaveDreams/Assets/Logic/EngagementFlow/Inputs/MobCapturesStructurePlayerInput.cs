using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCapturesStructurePlayerInput : PlayerInput
{
    public MapMob Capturing;
    public MapStructure Target;

    public MobCapturesStructurePlayerInput(MapMob capturing, MapStructure target)
    {
        Capturing = capturing;
        Target = target;
    }

    public override IEnumerator Execute(MapHolder mapHolder, MobHolder mobHolder)
    {
        if (!Capturing.CanCapture)
        {
            DebugTextLog.AddTextToLog("A unit tried to capture, but cannot capture");
            yield break;
        }

        if (Capturing.Position != Target.Position)
        {
            DebugTextLog.AddTextToLog("A unit tried to capture a structure they were not on the tile for");
            yield break;
        }

        Target.ProceedCapture(Capturing);
        Capturing.ExhaustAllOptions();
    }
}
