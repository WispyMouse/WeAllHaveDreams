using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLibrary : SingletonBase<TileLibrary>
{
    public GameplayTile DefaultTile;

    public static GameplayTile GetTile(string tileName)
    {
        return Singleton.DefaultTile;
    }
}
