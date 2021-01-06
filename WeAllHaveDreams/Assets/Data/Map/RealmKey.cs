using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RealmKey
{
    public const string Ownership = "Ownership";

    public string Color;
    public RealmKeyType Type;
    public string Object;

    public GameplayTile GetTileInstance()
    {
        return TileLibrary.GetTile(Object);
    }

    public MapStructure GetStructureInstance()
    {
        return StructureLibrary.GetStructure(Object);
    }

    public int GetTeam()
    {
        return int.Parse(Object);
    }
}
