using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureLibrary : SingletonBase<StructureLibrary>
{
    IEnumerable<StructureConfiguration> Configurations { get; set; } = System.Array.Empty<StructureConfiguration>();
    Dictionary<string, MapStructure> NamesToStructures { get; set; } = new Dictionary<string, MapStructure>();

    public Sprite[] BellSprites;
    public Sprite[] CapitalSprites;
    public Sprite[] SignpostSprites;

    public MapStructure DefaultStructure;
    public MapStructure StructureBase;

    void Awake()
    {
        ExplicitlySetSingleton();
    }

    public static IEnumerator LoadStructuresFromConfiguration()
    {
        DebugTextLog.AddTextToLog("Loading structures from configuration.", DebugTextLogChannel.DebugLogging);

        Singleton.Configurations = ConfigurationLoadingEntrypoint.GetConfigurationData<StructureConfiguration>();

        foreach (StructureConfiguration curConfiguration in Singleton.Configurations)
        {
            MapStructure thisStructure = Instantiate(Singleton.StructureBase);
            thisStructure.LoadFromConfiguration(curConfiguration);
            thisStructure.gameObject.SetActive(false);
            Singleton.NamesToStructures.Add(curConfiguration.DevelopmentName, thisStructure);
        }

        DebugTextLog.AddTextToLog($"Loaded {Singleton.Configurations.Count()} structures.", DebugTextLogChannel.DebugLogging);
        DebugTextLog.AddTextToLog($"Loaded structures: {string.Join(", ", Singleton.NamesToStructures.Values.Select(structure => structure.StructureName))}", DebugTextLogChannel.Verbose);
        yield break;
    }

    public static MapStructure GetStructure(string structureName)
    {
        if (string.IsNullOrEmpty(structureName))
        {
            return null;
        }

        if (Singleton.NamesToStructures.TryGetValue(structureName, out MapStructure foundStructure))
        {
            MapStructure newStructure = Instantiate(foundStructure);
            newStructure.LoadFromConfiguration(foundStructure.Configuration);
            newStructure.gameObject.SetActive(true);
            return newStructure;
        }

        DebugTextLog.AddTextToLog($"Could not find a Structure in the Library with the name {structureName}. Returning a default.", DebugTextLogChannel.RuntimeError);
        return Instantiate(Singleton.DefaultStructure);
    }

    public static IEnumerable<StructureConfiguration> GetAllStructures()
    {
        return Singleton.Configurations;
    }

    public static Sprite GetStructureSprite(string appearance, PlayerSide player)
    {
        return GetStructureSprite(appearance, player?.PlayerSideIndex);
    }

    public static Sprite GetStructureSprite(string appearance, int? side)
    {
        // TODO HACK: This is a temporary holdover spot so we don't need to figure out graphics loading
        switch (appearance)
        {
            case nameof(BellSprites):
                return Singleton.BellSprites[side.HasValue ? side.Value + 1 : 0];
            case nameof(CapitalSprites):
                return Singleton.CapitalSprites[side.HasValue ? side.Value + 1 : 0];
            case nameof(SignpostSprites):
                return Singleton.SignpostSprites[side.HasValue ? side.Value + 1 : 0];
            default:
                DebugTextLog.AddTextToLog($"Asked for {appearance} for a sprite, but that is not in the library. Returning default.", DebugTextLogChannel.RuntimeError);
                return Singleton.SignpostSprites[side.HasValue ? side.Value + 1 : 0];
        }
    }
}
