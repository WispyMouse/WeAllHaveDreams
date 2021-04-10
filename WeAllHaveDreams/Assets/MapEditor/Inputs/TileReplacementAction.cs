using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When invoked, places a <see cref="GameplayTile"/>.
/// </summary>
public class TileReplacementAction : MapEditorInput
{
    /// <summary>
    /// Position to apply the Tile.
    /// </summary>
    public Vector3Int Position;

    /// <summary>
    /// Name of the Tile to add.
    /// Certainly has a value.
    /// </summary>
    public string Added;

    /// <summary>
    /// Name of the Tile that was previously on <see cref="Position"/>.
    /// May be null if there was no tile present.
    /// </summary>
    public string Removed;

    /// <summary>
    /// Creates a new TileReplacementAction.
    /// </summary>
    /// <param name="position">Position to apply to.</param>
    /// <param name="added">Name of the tile to add.</param>
    /// <param name="worldContextInstance">The current WorldContext. Used to determine previous contents.</param>
    public TileReplacementAction(Vector3Int position, string added, WorldContext worldContextInstance)
    {
        Position = position;
        Added = added;

        Removed = worldContextInstance.MapHolder.GetGameplayTile(position)?.TileName;
    }

    /// <inheritdoc />
    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetTile(Position, TileLibrary.GetTile(Added));
    }

    /// <inheritdoc />
    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetTile(Position, TileLibrary.GetTile(Removed));
    }
}
