using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Realm
{
    public string Name = "Default";
    public IEnumerable<RealmCoordinate> RealmCoordinates = new RealmCoordinate[] { };
    public IEnumerable<StructureMapData> Structures = new StructureMapData[] { };

    public void Hydrate()
    {
    }

    public static Realm GetEmptyRealm()
    {
        Realm newRealm = new Realm();
        newRealm.RealmCoordinates = new List<RealmCoordinate>();
        return newRealm;
    }
}
