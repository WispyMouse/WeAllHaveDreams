using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogHolder : MonoBehaviour
{
    public Tilemap FogTileMap;

    public WorldContext WorldContextInstance;

    public Tile HiddenTile;
    public Tile HasBeenSeenTile;
    public Tile VisibleTile;

    public HashSet<Vector3Int> AllTiles { get; private set; } = new HashSet<Vector3Int>();
    public Dictionary<int, TeamVisibility> TeamVisibilityData = new Dictionary<int, TeamVisibility>();

    Configuration.FogVisibilityConfigurations fogVisibilityConfigurations { get; set; }

    public void Initialize()
    {
        fogVisibilityConfigurations = ConfigurationLoadingEntrypoint.GetConfigurationData<Configuration.FogVisibilityConfigurations>().First();
        DebugTextLog.AddTextToLog("Press F to switch visibility modes", DebugTextLogChannel.DebugLogging);

        AllTiles = new HashSet<Vector3Int>(WorldContextInstance.MapHolder.GetAllTiles());

        foreach (Vector3Int tile in AllTiles)
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
            foreach (Vector3Int position in AllTiles)
            {
                FogTileMap.SetTile(position, VisibleTile);
            }

            return;
        }

        var visibleTiles = new HashSet<Vector3Int>();
        var hasBeenTiles = new HashSet<Vector3Int>();

        foreach (int player in TeamVisibilityData.Keys)
        {
            if (fogVisibilityConfigurations.ShouldShowMapView(player))
            {
                TeamVisibility curVisibility = TeamVisibilityData[player];
                visibleTiles = new HashSet<Vector3Int>(visibleTiles.Union(curVisibility.VisibleTiles));
                hasBeenTiles = new HashSet<Vector3Int>(hasBeenTiles.Union(curVisibility.HasBeenSeenTiles));
            }
        }

        if (!fogVisibilityConfigurations.CoverMapInDarknessInitially)
        {
            hasBeenTiles = AllTiles;
        }

        foreach (Vector3Int position in AllTiles)
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
        foreach (PlayerSide curPlayer in TurnManager.GetPlayers())
        {
            UpdateVisibilityForPlayer(curPlayer.PlayerSideIndex);
        }
    }

    public void UpdateVisibilityForPlayer(int player)
    {
        TeamVisibility assignedVisibility;

        if (!TeamVisibilityData.TryGetValue(player, out assignedVisibility))
        {
            TeamVisibilityData.Add(player, assignedVisibility = new TeamVisibility());
        }

        assignedVisibility.ClearVisibleTiles();

        foreach (MapMob curMob in WorldContextInstance.MobHolder.MobsOnTeam(player))
        {
            Vector3Int mobPosition = curMob.Position;

            if (fogVisibilityConfigurations.UpdateVisibilityOnlyAfterSettling)
            {
                mobPosition = curMob.RestingPosition;
            }
            HashSet<Vector3Int> thisMobsVisibleTiles = CalculateVisibleTiles(curMob, mobPosition);

            assignedVisibility.IncorporateVisibleTiles(thisMobsVisibleTiles);
        }

        foreach (MapStructure curStructure in WorldContextInstance.StructureHolder.ActiveStructures.Where(structure => structure.PlayerSideIndex == player && !structure.UnCaptured))
        {
            HashSet<Vector3Int> thisStructureVisibleTiles = CalculateVisibleTiles(curStructure);
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

    public HashSet<Vector3Int> CalculateVisibleTiles(MapMob mob, Vector3Int fromPosition)
    {
        var seenPositions = new HashSet<Vector3Int>();

        var frontier = new HashSet<Vector3Int>();
        frontier.Add(fromPosition);
        seenPositions.Add(fromPosition);

        while (frontier.Any())
        {
            var thisTile = frontier.First();
            frontier.Remove(thisTile);

            // If we've already expended all our range, stop ranging (lol)
            int distanceFromStart = Mathf.Abs(fromPosition.x - thisTile.x) + Mathf.Abs(fromPosition.y - thisTile.y);
            if (distanceFromStart >= mob.SightRange)
            {
                continue;
            }

            foreach (Vector3Int neighbor in WorldContextInstance.MapHolder.GetNeighbors(thisTile))
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

    public HashSet<Vector3Int> CalculateVisibleTiles(MapStructure mapStructure)
    {
        return new HashSet<Vector3Int>() { mapStructure.Position };
    }

    public bool ClearLineOfVisibility(Vector3Int pointA, Vector3Int pointB)
    {
        foreach (Vector3Int point in BresenhamLineDrawer.PointsOnLine(pointA, pointB))
        {
            // TODO: Obstructions!
        }

        return true;
    }

    public void ClearAllTiles()
    {
        AllTiles = new HashSet<Vector3Int>();
        TeamVisibilityData = new Dictionary<int, TeamVisibility>();
        FogTileMap.ClearAllTiles();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
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

    public bool PointIsVisibleToPlayer(Vector3Int point, int player)
    {
        return TeamVisibilityData[player].VisibleTiles.Contains(point);
    }

    public bool PointIsVisibleToCurrentPerspective(Vector3Int point)
    {
        switch (fogVisibilityConfigurations.FogTurnHandlingMode)
        {
            case FogTurnHandlingEnum.ShowAllMap:
                return true;
            case FogTurnHandlingEnum.ShowAllVisibility:
                return TurnManager.GetPlayers().Any(player => PointIsVisibleToPlayer(point, player.PlayerSideIndex));
            case FogTurnHandlingEnum.SwitchEachTurn:
                return PointIsVisibleToPlayer(point, TurnManager.CurrentPlayer.PlayerSideIndex);
            case FogTurnHandlingEnum.StayOnOnePlayer:
                return PointIsVisibleToPlayer(point, fogVisibilityConfigurations.FactionToShowFogFor);
            default:
                DebugTextLog.AddTextToLog($"Unrecognized FogVisibilityConfiguration for PointIsVisibleToCurrentPerspective");
                return false;
        }
    }

    public void ManageVisibilityToCurrentPerspective(MapObject toConsider)
    {
        ManageVisibilityToCurrentPerspective(toConsider, toConsider.Position);
    }

    public void ManageVisibilityToCurrentPerspective(MapObject toConsider, Vector3Int position)
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
