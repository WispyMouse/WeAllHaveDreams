using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Realm
{
    public string Name = "Default";
    public IEnumerable<RealmCoordinate> RealmCoordinates = Array.Empty<RealmCoordinate>();
    public IEnumerable<StructureMapData> Structures = Array.Empty<StructureMapData>();
    public IEnumerable<MobMapData> Mobs = Array.Empty<MobMapData>();

    public void Hydrate()
    {
    }

    public static Realm GetEmptyRealm()
    {
        Realm newRealm = new Realm();
        return newRealm;
    }
}
