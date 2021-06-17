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

    public Sprite[] BasicMobSprite;
    public Sprite[] RangedMobSprite;
    public Sprite[] EngineerMobSprite;

    public static IEnumerator LoadMobsFromConfiguration()
    {
        DebugTextLog.AddTextToLog("Loading mobs from configuration.", DebugTextLogChannel.DebugLogging);

        Singleton.MobConfigurations = ConfigurationLoadingEntrypoint.GetConfigurationData<MobConfiguration>();

        foreach (MobConfiguration curConfiguration in Singleton.MobConfigurations)
        {
            MapMob thisMob = Instantiate(Singleton.MobBase);
            thisMob.LoadFromConfiguration(curConfiguration);
            thisMob.gameObject.SetActive(false);
            Singleton.NamesToMobs.Add(thisMob.DevelopmentName, thisMob);
        }

        DebugTextLog.AddTextToLog($"Loaded {Singleton.MobConfigurations.Count()} mobs.", DebugTextLogChannel.DebugLogging);
        DebugTextLog.AddTextToLog($"Loaded mobs: {string.Join(", ", Singleton.MobConfigurations.Select(mob => mob.Name))}", DebugTextLogChannel.Verbose);
        yield break;
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

    public static Sprite GetMobSprite(string appearance, PlayerSide side)
    {
        if (side == null)
        {
            return null;
        }

        return GetMobSprite(appearance, side.PlayerSideIndex);
    }

    public static Sprite GetMobSprite(string appearance, int side)
    {
        // TODO HACK: This is a temporary holdover spot so we don't need to figure out graphics loading
        switch (appearance)
        {
            case nameof(BasicMobSprite):
                return Singleton.BasicMobSprite[side];
            case nameof(RangedMobSprite):
                return Singleton.RangedMobSprite[side];
            case nameof(EngineerMobSprite):
                return Singleton.EngineerMobSprite[side];
            default:
                DebugTextLog.AddTextToLog($"Asked for {appearance} for a sprite, but that is not in the library. Returning default.", DebugTextLogChannel.RuntimeError);
                return Singleton.BasicMobSprite[side];
        }
    }

    public static IEnumerable<MobConfiguration> MobsWithTags(IEnumerable<string> tags)
    {
        return Singleton.MobConfigurations.Where(mob => tags.All(tag => mob.Tags.Contains(tag)));
    }

    public static IEnumerable<MobConfiguration> GetAllMobs()
    {
        return Singleton.MobConfigurations;
    }
}
