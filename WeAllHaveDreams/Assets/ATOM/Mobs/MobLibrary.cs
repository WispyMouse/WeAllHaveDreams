using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobLibrary : SingletonBase<MobLibrary>
{
    public MapMob[] Mobs;
    Dictionary<string, MapMob> NamesToMobs { get; set; } = new Dictionary<string, MapMob>();

    public MapMob DefaultMob;

    public static MapMob GetMob(string mobName)
    {
        if (Singleton.NamesToMobs.TryGetValue(mobName, out MapMob foundMob))
        {
            return foundMob;
        }

        MapMob matchingMob = Singleton.Mobs.FirstOrDefault(mob => mob.name == mobName);

        if (matchingMob == null)
        {
            DebugTextLog.AddTextToLog($"Could not find a Mob in the Library with the name {mobName}. Returning a default.");
            return Singleton.DefaultMob;
        }

        Singleton.NamesToMobs.Add(mobName, matchingMob);
        return matchingMob;
    }
}
