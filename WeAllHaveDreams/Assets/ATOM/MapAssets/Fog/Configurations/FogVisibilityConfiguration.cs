using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    public class FogVisibilityConfigurations : ConfigurationData
    {
        public int FactionToShowFogFor;
        public FogTurnHandlingEnum FogTurnHandlingMode;
        public bool CoverMapInDarknessInitially;

        public FogVisibilityConfigurations() : base()
        {
            FactionToShowFogFor = 0;
            FogTurnHandlingMode = FogTurnHandlingEnum.ShowAllMap;
            CoverMapInDarknessInitially = false;
        }

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
}
