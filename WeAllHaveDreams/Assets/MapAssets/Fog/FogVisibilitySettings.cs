using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FogVisibilitySettings
{
    public int FactionToShowFogFor;
    public FogTurnHandlingEnum FogTurnHandlingMode;
    public bool CoverMapInDarknessInitially;

    public bool ShouldShowMapView(int player)
    {
        switch (FogTurnHandlingMode)
        {
            case FogTurnHandlingEnum.StayOnOnePlayer:
                return player == FactionToShowFogFor;
            case FogTurnHandlingEnum.SwitchEachTurn:
                return player == TurnManager.CurrentPlayer.PlayerSideIndex;
            case FogTurnHandlingEnum.ShowAllVisibility:
            case FogTurnHandlingEnum.ShowAllMap:
                return true;
            default:
                DebugTextLog.AddTextToLog($"FogTurnHandlingEnum set to unhandled value for showing ShouldShowMapView: {FogTurnHandlingMode}");
                return false;
        }
    }
}
