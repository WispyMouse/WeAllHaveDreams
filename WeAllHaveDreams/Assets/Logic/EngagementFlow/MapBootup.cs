using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class MapBootup : MonoBehaviour
{
    const string MapFolderPathRelative = "Resources/Maps";
    public static string MapFolderPath
    {
        get
        {
            return Path.Combine(Application.dataPath, MapFolderPathRelative).Replace("\\", "/");
        }
    }

    const string MapFileSuffix = ".json";
    static string MapFileSearch
    {
        get
        {
            return $"*{MapFileSuffix}";
        }
    }

    public TurnManager TurnManagerInstance;
    public MapHolder MapHolderInstance;

    private async void Start()
    {
        await ConfigurationLoadingEntrypoint.LoadAllConfigurationData();
        Realm defaultRealm = await GetDefaultRealm();
        await MapHolderInstance.LoadFromRealm(defaultRealm);
        TurnManagerInstance.GameplayReady();
    }

    async Task<Realm> GetDefaultRealm()
    {
        foreach (string filePath in Directory.GetFiles(MapFolderPath, MapFileSearch, SearchOption.AllDirectories))
        {
            string sanitizedFilePath = filePath.Replace("\\", "/");

            // HACK TODO: This just gets the first one it finds and calls it good
            string fileText;

            using (var reader = File.OpenText(sanitizedFilePath))
            {
                fileText = await reader.ReadToEndAsync();
            }

            return JsonUtility.FromJson<Realm>(fileText);
        }

        return null;
    }
}
