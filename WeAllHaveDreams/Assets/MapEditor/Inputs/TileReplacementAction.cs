using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReplacementAction : MapEditorInput
{
    public Vector3Int Position;

    public string Removed { get; set; }
    public string Added { get; set; }

    public TileReplacementAction(Vector3Int position, string removed, string added)
    {
        Position = position;
        Removed = removed;
        Added = added;
    }

    public override void Invoke(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetTile(Position, TileLibrary.GetTile(Added));
    }

    public override void Undo(WorldContext worldContextInstance)
    {
        worldContextInstance.MapHolder.SetTile(Position, TileLibrary.GetTile(Removed));
    }
}
