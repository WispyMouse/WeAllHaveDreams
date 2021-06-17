using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogHolder : MonoBehaviour
{
    public Tilemap FogTileMap;

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public Tile HiddenTile;
    public Tile HasBeenSeenTile;
    public Tile VisibleTile;

    public HashSet<MapCoordinates> AllTiles { get; private set; } = new HashSet<MapCoordinates>();
    public Dictionary<PlayerSide, TeamVisibility> TeamVisibilityData = new Dictionary<PlayerSide, TeamVisibility>();

    Configuration.FogVisibilityConfigurations fogVisibilityConfigurations { get; set; }

    public void Initialize()
    {
        fogVisibilityConfigurations = ConfigurationLoadingEntrypoint.GetConfigurationData<Configuration.FogVisibilityConfigurations>().First();
        DebugTextLog.AddTextToLog("Press F to switch visibility modes", DebugTextLogChannel.DebugLogging);

        AllTiles = new HashSet<MapCoordinates>(WorldContextInstance.MapHolder.GetAllTiles());

        foreach (MapCoordinates tile in AllTiles)
        {
            if (fogVisibilityConfigurations.CoverMapInDarknessInitially)
            {
                FogTileMap.SetTile(tile, HiddenTile);
            }
            else
            {
                FogTileMap.SetTile(tile, HasBeenSeenTile);
            }
        }
    }

    public void RefreshFogVisuals()
    {
        if (fogVisibilityConfigurations.FogTurnHandlingMode == FogTurnHandlingEnum.ShowAllMap)
        {
            foreach (MapCoordinates position in AllTiles)
            {
                FogTileMap.SetTile(position, VisibleTile);
            }

            return;
        }

        var visibleTiles = new HashSet<MapCoordinates>();
        var hasBeenTiles = new HashSet<MapCoordinates>();

        foreach (PlayerSide player in TeamVisibilityData.Keys)
        {
            if (fogVisibilityConfigurations.ShouldShowMapView(player))
            {
                TeamVisibility curVisibility = TeamVisibilityData[player];
                visibleTiles = new HashSet<MapCoordinates>(visibleTiles.Union(curVisibility.VisibleCoordinates));
                hasBeenTiles = new HashSet<MapCoordinates>(hasBeenTiles.Union(curVisibility.HasBeenSeenCoordinates));
            }
        }

        if (!fogVisibilityConfigurations.CoverMapInDarknessInitially)
        {
            hasBeenTiles = AllTiles;
        }

        foreach (MapCoordinates position in AllTiles)
        {
            if (visibleTiles.Contains(position))
            {
                FogTileMap.SetTile(position, VisibleTile);
            }
            else if (hasBeenTiles.Contains(position))
            {
                FogTileMap.SetTile(position, HasBeenSeenTile);
            }
            else
            {
                FogTileMap.SetTile(position, HiddenTile);
            }
        }
    }

    public void UpdateVisibilityForPlayers()
    {
        foreach (PlayerSide curPlayer in FactionHolder.GetPlayers())
        {
            UpdateVisibilityForPlayer(curPlayer);
        }
    }

    public void UpdateVisibilityForPlayer(PlayerSide player)
    {
        TeamVisibility assignedVisibility;

        if (!TeamVisibilityData.TryGetValue(player, out assignedVisibility))
        {
            TeamVisibilityData.Add(player, assignedVisibility = new TeamVisibility());
        }

        assignedVisibility.ClearVisibleTiles();

        foreach (MapMob curMob in WorldContextInstance.MobHolder.MobsOnTeam(player))
        {
            MapCoordinates mobPosition = curMob.Position;

            if (fogVisibilityConfigurations.UpdateVisibilityOnlyAfterSettling)
            {
                mobPosition = curMob.RestingPosition;
            }
            HashSet<MapCoordinates> thisMobsVisibleTiles = CalculateVisibleTiles(curMob, mobPosition);

            assignedVisibility.IncorporateVisibleTiles(thisMobsVisibleTiles);
        }

        foreach (MapStructure curStructure in WorldContextInstance.StructureHolder.ActiveStructures.Where(structure => structure.MyPlayerSide == player))
        {
            HashSet<MapCoordinates> thisStructureVisibleTiles = CalculateVisibleTiles(curStructure);
            assignedVisibility.IncorporateVisibleTiles(thisStructureVisibleTiles);
        }

        foreach (MapMob curMob in WorldContextInstance.MobHolder.ActiveMobs)
        {
            ManageVisibilityToCurrentPerspective(curMob);
        }

        if (fogVisibilityConfigurations.ShouldShowMapView(player))
        {
            RefreshFogVisuals();
        }
    }

    public HashSet<MapCoordinates> CalculateVisibleTiles(MapMob mob, MapCoordinates fromPosition)
    {
        var seenPositions = new HashSet<MapCoordinates>();
        var frontier = new HashSet<MapCoordinates>();

        frontier.Add(fromPosition);
        seenPositions.Add(fromPosition);

        while (frontier.Any())
        {
            var thisTile = frontier.First();
            frontier.Remove(thisTile);

            // If we've already expended all our range, stop ranging (lol)
            int distanceFromStart = Mathf.Abs(fromPosition.X - thisTile.X) + Mathf.Abs(fromPosition.Y - thisTile.Y);
            if (distanceFromStart >= mob.SightRange)
            {
                continue;
            }

            foreach (MapCoordinates neighbor in WorldContextInstance.MapHolder.GetNeighbors(thisTile))
            {
                if (seenPositions.Contains(neighbor))
                {
                    continue;
                }

                if (ClearLineOfVisibility(fromPosition, neighbor))
                {
                    seenPositions.Add(neighbor);
                    frontier.Add(neighbor);
                }
            }
        }

        return seenPositions;
    }

    public HashSet<MapCoordinates> CalculateVisibleTiles(MapStructure mapStructure)
    {
        return new HashSet<MapCoordinates>() { mapStructure.Position };
    }

    public bool ClearLineOfVisibility(MapCoordinates pointA, MapCoordinates pointB)
    {
        bool obstructedOnce = false;

        foreach (MapCoordinates point in BresenhamLineDrawer.PointsOnLine(pointA, pointB))
        {
            GameplayTile tile;

            if (tile = WorldContextInstance.MapHolder.GetGameplayTile(point))
            {
                if (tile.Configuration.ObstructsVision)
                {
                    if (obstructedOnce)
                    {
                        return false;
                    }

                    obstructedOnce = true;
                }
            }
        }

        return true;
    }

    public void ClearAllTiles()
    {
        AllTiles = new HashSet<MapCoordinates>();
        TeamVisibilityData = new Dictionary<PlayerSide, TeamVisibility>();
        FogTileMap.ClearAllTiles();
    }

    private void Update()
    {
        if (TurnManager.GameIsInProgress && Input.GetKeyDown(KeyCode.F))
        {
            FogTurnHandlingEnum previousSetting = fogVisibilityConfigurations.FogTurnHandlingMode;
            List<FogTurnHandlingEnum> allSettings = Enum.GetValues(typeof(FogTurnHandlingEnum)).Cast<FogTurnHandlingEnum>().ToList();
            int previousIndex = allSettings.IndexOf(previousSetting);
            int nextIndex = (previousIndex + 1) % allSettings.Count;
            FogTurnHandlingEnum nextSetting = allSettings[nextIndex];
            fogVisibilityConfigurations.FogTurnHandlingMode = nextSetting;

            DebugTextLog.AddTextToLog($"Changing fog settings from {previousSetting} to {nextSetting}.", DebugTextLogChannel.DebugOperations);
            UpdateVisibilityForPlayers();
        }
    }

    public bool PointIsVisibleToPlayer(MapCoordinates point, PlayerSide player)
    {
        return TeamVisibilityData[player].VisibleCoordinates.Contains(point);
    }

    public bool PointIsVisibleToCurrentPerspective(MapCoordinates point)
    {
        switch (fogVisibilityConfigurations.FogTurnHandlingMode)
        {
            case FogTurnHandlingEnum.ShowAllMap:
                return true;
            case FogTurnHandlingEnum.ShowAllVisibility:
                return FactionHolder.GetPlayers().Any(player => PointIsVisibleToPlayer(point, player));
            case FogTurnHandlingEnum.SwitchEachTurn:
                return PointIsVisibleToPlayer(point, TurnManager.CurrentPlayer);
            case FogTurnHandlingEnum.StayOnOnePlayer:
                return PointIsVisibleToPlayer(point, FactionHolder.GetPlayer(fogVisibilityConfigurations.FactionToShowFogFor));
            default:
                DebugTextLog.AddTextToLog($"Unrecognized FogVisibilityConfiguration for PointIsVisibleToCurrentPerspective");
                return false;
        }
    }

    public void ManageVisibilityToCurrentPerspective(MapObject toConsider)
    {
        ManageVisibilityToCurrentPerspective(toConsider, toConsider.Position);
    }

    public void ManageVisibilityToCurrentPerspective(MapObject toConsider, MapCoordinates position)
    {
        if (!PointIsVisibleToCurrentPerspective(position))
        {
            toConsider.HideDueToFog();
        }
        else
        {
            toConsider.BeVisible();
        }
    }
}
