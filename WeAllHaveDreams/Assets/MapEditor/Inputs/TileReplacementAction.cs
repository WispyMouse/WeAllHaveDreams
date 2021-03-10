using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileReplacementAction
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
}
