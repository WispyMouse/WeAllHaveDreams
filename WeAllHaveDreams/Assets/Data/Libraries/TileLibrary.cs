using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileLibrary : SingletonBase<TileLibrary>
{
    IEnumerable<TileConfiguration> Configurations { get; set; } = System.Array.Empty<TileConfiguration>();
    Dictionary<string, GameplayTile> NamesToTiles { get; set; } = new Dictionary<string, GameplayTile>();

    public Sprite[] TileSprites;

    public GameplayTile TileBase;
    public GameplayTile DefaultTile;

    void Awake()
    {
        ExplicitlySetSingleton();
    }

    public static IEnumerator LoadTilesFromConfiguration()
    {
        DebugTextLog.AddTextToLog("Loading tiles from configuration.", DebugTextLogChannel.DebugLogging);

        Singleton.Configurations = ConfigurationLoadingEntrypoint.GetConfigurationData<TileConfiguration>();

        foreach (TileConfiguration curConfiguration in Singleton.Configurations)
        {
            GameplayTile tileInstance = ScriptableObject.CreateInstance<GameplayTile>();
            tileInstance.LoadFromConfiguration(curConfiguration);
            Singleton.NamesToTiles.Add(curConfiguration.TileName, tileInstance);
        }

        DebugTextLog.AddTextToLog($"Loaded {Singleton.Configurations.Count()} tiles.", DebugTextLogChannel.ConfigurationReport);
        DebugTextLog.AddTextToLog($"Loaded tiles: {string.Join(", ", Singleton.NamesToTiles.Values.Select(structure => structure.Configuration.TileName))}", DebugTextLogChannel.Verbose);
        yield break;
    }

    public static GameplayTile GetTile(string tileName)
    {
        if (string.IsNullOrEmpty(tileName))
        {
            return null;
        }

        if (Singleton.NamesToTiles.TryGetValue(tileName, out GameplayTile foundTile))
        {
            return foundTile;
        }

        DebugTextLog.AddTextToLog($"Could not find a Tile in the Library with the name {tileName}. Returning a default.", DebugTextLogChannel.RuntimeError);
        return Instantiate(Singleton.DefaultTile);
    }

    public static IEnumerable<TileConfiguration> GetAllTiles()
    {
        return Singleton.Configurations;
    }

    public static Sprite GetSprite(string spriteName)
    {
        Sprite foundSprite = Singleton.TileSprites.FirstOrDefault(sprite => sprite.name == spriteName);

        if (foundSprite == null)
        {
            DebugTextLog.AddTextToLog($"Could not find a Tile sprite in the Library with the name {spriteName}. Returning a default.", DebugTextLogChannel.RuntimeError);
        }

        return foundSprite;
    }
}
