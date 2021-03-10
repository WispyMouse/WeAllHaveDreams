using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBootup : MonoBehaviour
{
    public const string MapFolderPathRelative = "Resources/Maps";
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
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    private async Task Start()
    {
        DebugTextLog.AddTextToLog("Loading Library");
        AsyncOperation libraryHandle = SceneManager.LoadSceneAsync("Library", LoadSceneMode.Additive);
        while (!libraryHandle.isDone)
        {
            await Task.Delay(1);
        }

        DebugTextLog.AddTextToLog("Loading WorldContext");
        AsyncOperation contextHandle = SceneManager.LoadSceneAsync("WorldContext", LoadSceneMode.Additive);
        while (!contextHandle.isDone)
        {
            await Task.Delay(1);
        }

        await ConfigurationLoadingEntrypoint.LoadAllConfigurationData();
        Realm defaultRealm = await GetDefaultRealm();
        await WorldContextInstance.MapHolder.LoadFromRealm(defaultRealm);
        TurnManagerInstance.GameplayReady();

        DebugTextLog.AddTextToLog("Press M to enter Map Editor mode", DebugTextLogChannel.DebugOperationInputInstructions);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TransitionToMapEditor();
        }
    }

    public static async Task<Realm> GetDefaultRealm()
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

            return JsonConvert.DeserializeObject<Realm>(fileText);
        }

        return null;
    }

    public void TransitionToMapEditor()
    {
        TurnManager.StopAllInputs();
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }
}
