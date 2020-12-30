using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RealmKey
{
    public string Color;
    public string Object;

    public GameplayTile GetTileInstance()
    {
        return TileLibrary.GetTile(Object);
    }
}
