using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodFillTilePaintOption : PaletteOptions
{
    /// <summary>
    /// How many tiles can be filled in by one flood operation at once.
    /// If we've exceeded this number, the operation is canceled.
    /// </summary>
    const int FloodMaxCrawl = 999;

    public override string OptionsName => "Flood Fill Tiles";
    public override IEnumerable<string> ExclusiveTags => new string[] { PaletteOptionTags.TilePaintClickOperation };
    public override IEnumerable<Type> RelevantPaletteSettings => new Type[] { typeof(TileReplacementAction) };

    public override void Apply(WorldContext worldContextInstance, MapEditorInput toApply)
    {
        TileReplacementAction tileReplacement = toApply as TileReplacementAction;
        if (tileReplacement == null)
        {
            return;
        }

        if (tileReplacement.Positions.Count != 1)
        {
            DebugTextLog.AddTextToLog($"{OptionsName} cannot be applied to a {nameof(TileReplacementAction)} with not-one Positions, but input had {tileReplacement.Positions.Count}", DebugTextLogChannel.MapEditorOperations);
            return;
        }

        MapCoordinates seedCoordinate = tileReplacement.Positions.First();
        HashSet<MapCoordinates> visitedCoordinates = new HashSet<MapCoordinates>();
        HashSet<MapCoordinates> frontier = new HashSet<MapCoordinates>() { seedCoordinate };

        // Select tiles on the map that match this one
        GameplayTile targetTile = worldContextInstance.MapHolder.GetGameplayTile(seedCoordinate);

        while (frontier.Any())
        {
            MapCoordinates thisCoordinate = frontier.First();
            frontier.Remove(thisCoordinate);
            visitedCoordinates.Add(thisCoordinate);

            IEnumerable<MapCoordinates> neighbors = GameMap.GetPotentialNeighbors(thisCoordinate);
            foreach (MapCoordinates neighbor in neighbors)
            {
                GameplayTile matchingTile = worldContextInstance.MapHolder.GetGameplayTile(neighbor);

                if (visitedCoordinates.Contains(neighbor))
                {
                    continue; // Already visited, skip
                }

                if (targetTile == null)
                {
                    if (matchingTile != null)
                    {
                        continue; // We wanted null and this isn't null, skip
                    }
                }
                else
                {
                    if (matchingTile == null)
                    {
                        continue; // We wanted something and this is void, skip
                    }

                    if (targetTile.TileName != matchingTile.TileName)
                    {
                        continue; // We wanted something else, skip
                    }
                }

                // If we made it here, then this should be included in the flood fill
                frontier.Add(neighbor);
            }

            if (visitedCoordinates.Count > FloodMaxCrawl)
            {
                DebugTextLog.AddTextToLog($"Flood fill operation cancelled: Exceeded limit of {FloodMaxCrawl} tiles");
                return;
            }
        }

        tileReplacement.Positions = visitedCoordinates;
        tileReplacement.Removed = tileReplacement.GetRemovedTiles(worldContextInstance);
    }
}
