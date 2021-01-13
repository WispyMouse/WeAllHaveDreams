using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MobLibrary : SingletonBase<MobLibrary>
{
    IEnumerable<MobConfiguration> MobConfigurations { get; set; }
    Dictionary<string, MapMob> NamesToMobs { get; set; } = new Dictionary<string, MapMob>();

    public MapMob MobBase;

    public async static Task LoadMobsFromConfiguration()
    {
        Singleton.MobConfigurations = ConfigurationLoadingEntrypoint.GetConfigurationData<MobConfiguration>();

        foreach (MobConfiguration curConfiguration in Singleton.MobConfigurations)
        {
            MapMob thisMob = Instantiate(Singleton.MobBase);
            thisMob.LoadFromConfiguration(curConfiguration);
            thisMob.gameObject.SetActive(false);
            Singleton.NamesToMobs.Add(thisMob.DevelopmentName, thisMob);
        }
    }

    public static MapMob GetMob(string mobName)
    {
        if (Singleton.NamesToMobs.TryGetValue(mobName, out MapMob foundMob))
        {
            return foundMob;
        }

        DebugTextLog.AddTextToLog($"Could not find a Mob in the Library with the name {mobName}.", DebugTextLogChannel.ConfigurationError);
        return null;
    }
}
