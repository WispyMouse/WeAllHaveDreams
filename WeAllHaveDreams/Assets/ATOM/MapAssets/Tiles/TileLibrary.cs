﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileLibrary : SingletonBase<TileLibrary>
{
    public const string Tile = "Tile";

    public GameplayTile[] Tiles;
    Dictionary<string, GameplayTile> NamesToTiles { get; set; } = new Dictionary<string, GameplayTile>();

    public GameplayTile DefaultTile;

    public static GameplayTile GetTile(string tileName)
    {
        if (Singleton.NamesToTiles.TryGetValue(tileName, out GameplayTile foundTile))
        {
            return foundTile;
        }

        GameplayTile matchingTile = Singleton.Tiles.FirstOrDefault(tile => tile.name == tileName);

        if (matchingTile == null)
        {
            DebugTextLog.AddTextToLog($"Could not find a Tile in the Library with the name {tileName}. Returning a default.");
            return Singleton.DefaultTile;
        }

        Singleton.NamesToTiles.Add(tileName, matchingTile);
        return matchingTile;
    }
}
