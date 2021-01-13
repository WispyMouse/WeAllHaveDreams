using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    public class FogVisibilityConfigurations : ConfigurationData
    {
        public int FactionToShowFogFor { get; set; }
        public bool CoverMapInDarknessInitially { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FogTurnHandlingEnum FogTurnHandlingMode { get; set; }
        
        public FogVisibilityConfigurations() : base()
        {
        }

        public override string GetConfigurationShortReport()
        {
            return $"Mode: {FogTurnHandlingMode} // Faction: {FactionToShowFogFor} // Initial Darkness: {CoverMapInDarknessInitially}";
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
