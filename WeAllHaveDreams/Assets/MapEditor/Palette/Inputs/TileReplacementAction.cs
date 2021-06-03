using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When invoked, places a <see cref="GameplayTile"/>.
/// </summary>
public class TileReplacementAction : MapEditorInput
{
    /// <summary>
    /// Positions to paint on.
    /// </summary>
    public HashSet<MapCoordinates> Positions;

    /// <summary>
    /// Name of the Tile to add.
    /// Certainly has a value.
    /// </summary>
    public string Added;

    /// <summary>
    /// A lookup table of previously coordinates, and what tile was previously there.
    /// The keys of this should only contain values in <see cref="Positions"/>. Not every value in Positions will be in Removed if the tile was empty before.
    /// </summary>
    public Dictionary<MapCoordinates, string> Removed;

    /// <summary>
    /// Creates a new TileReplacementAction.
    /// </summary>
    /// <param name="position">Position to apply to.</param>
    /// <param name="added">Name of the tile to add.</param>
    /// <param name="worldContextInstance">The current WorldContext. Used to determine previous contents.</param>
    public TileReplacementAction(MapCoordinates position, string added, WorldContext worldContextInstance)
    {
        Positions = new HashSet<MapCoordinates>() { position };
        Added = added;

        Removed = GetRemovedTiles(worldContextInstance);
    }

    public Dictionary<MapCoordinates, string> GetRemovedTiles(WorldContext worldContextInstance)
    {
        Dictionary<MapCoordinates, string> removed = new Dictionary<MapCoordinates, string>();

        foreach (MapCoordinates coordinate in Positions)
        {
            GameplayTile tile = worldContextInstance.MapHolder.GetGameplayTile(coordinate);

            if (tile != null)
            {
                removed.Add(coordinate, tile.TileName);
            }
        }

        return removed;
    }

    /// <inheritdoc />
    public override void Invoke(WorldContext worldContextInstance)
    {
        GameplayTile tilePf = TileLibrary.GetTile(Added);

        foreach (MapCoordinates coordinate in Positions)
        {
            worldContextInstance.MapHolder.SetTile(coordinate, tilePf);
        }
    }

    /// <inheritdoc />
    public override void Undo(WorldContext worldContextInstance)
    {
        foreach (MapCoordinates coordinate in Positions)
        {
            if (Removed.ContainsKey(coordinate))
            {
                worldContextInstance.MapHolder.SetTile(coordinate, TileLibrary.GetTile(Removed[coordinate]));
            }
            else
            {
                worldContextInstance.MapHolder.SetTile(coordinate, null);
            }
        }
    }
}
