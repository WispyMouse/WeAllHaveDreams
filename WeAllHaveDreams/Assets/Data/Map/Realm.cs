using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Realm
{
    public string Name = "Default";
    public IEnumerable<RealmCoordinate> RealmCoordinates;

    public async Task Hydrate()
    {
        await Task.Delay(0);
    }

    public static Realm GetEmptyRealm()
    {
        Realm newRealm = new Realm();
        newRealm.RealmCoordinates = new List<RealmCoordinate>();
        return newRealm;
    }
}
