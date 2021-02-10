using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RealmKey
{
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
    
    public MapMob GetMobPrefab()
    {
        return MobLibrary.GetMob(Object);
    }

    public MapFeature GetFeatureInstance()
    {
        return FeatureLibrary.GetFeature(Object);
    }

    public int GetTeam()
    {
        return int.Parse(Object);
    }
}
