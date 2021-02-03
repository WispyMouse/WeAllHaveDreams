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

    public override string LongTitle => $"Capture a structure with {Capturing.name} at ({Target.Position.x}, {Target.Position.y}) ";

    public override IEnumerator Execute(WorldContext worldContext)
    {
        if (Capturing.Position != Target.Position)
        {
            if (!Capturing.CanMove)
            {
                DebugTextLog.AddTextToLog($"A unit tried to move to capture {Target.Position.x}, {Target.Position.y}, but can't move");
                yield break;
            }

            if (worldContext.MobHolder.MobOnPoint(Target.Position))
            {
                DebugTextLog.AddTextToLog($"A unit tried to move to capture {Target.Position.x}, {Target.Position.y}, but there was already a different unit on that point");
                yield break;
            }

            yield return worldContext.MobHolder.MoveUnit(Capturing, Target.Position);
            Capturing.CanMove = false;
        }

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
