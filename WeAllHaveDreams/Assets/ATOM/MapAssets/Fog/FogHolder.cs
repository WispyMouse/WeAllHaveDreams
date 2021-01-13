using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogHolder : MonoBehaviour
{
    public Tilemap FogTileMap;

    public MapHolder MapHolderController;

    public Tile HiddenTile;
    public Tile HasBeenSeenTile;
    public Tile VisibleTile;

    public HashSet<Vector3Int> AllTiles { get; private set; } = new HashSet<Vector3Int>();
    public Dictionary<int, TeamVisibility> TeamVisibilityData = new Dictionary<int, TeamVisibility>();

    Configuration.FogVisibilityConfigurations fogVisibilityConfigurations { get; set; }

    public void Initialize(MapHolder mapHolderInstance)
    {
        fogVisibilityConfigurations = ConfigurationLoadingEntrypoint.GetConfigurationData<Configuration.FogVisibilityConfigurations>().First();

        AllTiles = new HashSet<Vector3Int>(mapHolderInstance.GetAllTiles());

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

    public void UpdateVisibilityForPlayers(MapHolder mapHolderInstance, MobHolder mobHolderInstance)
    {
        foreach (PlayerSide curPlayer in TurnManager.GetPlayers())
        {
            UpdateVisibilityForPlayer(curPlayer.PlayerSideIndex, mapHolderInstance, mobHolderInstance);
        }
    }

    public void UpdateVisibilityForPlayer(int player, MapHolder mapHolderInstance, MobHolder mobHolderInstance)
    {
        TeamVisibility assignedVisibility;

        if (!TeamVisibilityData.TryGetValue(player, out assignedVisibility))
        {
            TeamVisibilityData.Add(player, assignedVisibility = new TeamVisibility());
        }

        assignedVisibility.ClearVisibleTiles();

        foreach (MapMob curMob in mobHolderInstance.MobsOnTeam(player))
        {
            HashSet<Vector3Int> thisMobsVisibleTiles = CalculateVisibleTiles(curMob, mapHolderInstance);
            assignedVisibility.IncorporateVisibleTiles(thisMobsVisibleTiles);
        }

        if (fogVisibilityConfigurations.ShouldShowMapView(player))
        {
            RefreshFogVisuals();
        }
    }

    public HashSet<Vector3Int> CalculateVisibleTiles(MapMob mob, MapHolder mapHolderInstance)
    {
        var seenPositions = new HashSet<Vector3Int>();

        var frontier = new HashSet<Vector3Int>();
        frontier.Add(mob.Position);
        seenPositions.Add(mob.Position);

        while (frontier.Any())
        {
            var thisTile = frontier.First();
            frontier.Remove(thisTile);

            // If we've already expended all our range, stop ranging (lol)
            int distanceFromStart = Mathf.Abs(mob.Position.x - thisTile.x) + Mathf.Abs(mob.Position.y - thisTile.y);
            if (distanceFromStart >= mob.SightRange)
            {
                continue;
            }

            foreach (Vector3Int neighbor in mapHolderInstance.GetNeighbors(thisTile))
            {
                if (seenPositions.Contains(neighbor))
                {
                    continue;
                }

                if (ClearLineOfVisibility(mob.Position, neighbor))
                {
                    seenPositions.Add(neighbor);
                    frontier.Add(neighbor);
                }
            }
        }

        return seenPositions;
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
}
