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

    public static Realm WIPRealm; // HACK: Putting something in here will make it load up, instead of the Default Realm

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

        Realm realmToLoad = WIPRealm;
        if (realmToLoad == null)
        {
            DebugTextLog.AddTextToLog("Loading default realm", DebugTextLogChannel.DebugLogging);
            realmToLoad = await GetDefaultRealm();
        }

        await WorldContextInstance.MapHolder.LoadFromRealm(realmToLoad);
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

        DebugTextLog.AddTextToLog("Attempted to load default realm, but could not find any, returning empty", DebugTextLogChannel.DebugLogging);

        return Realm.GetEmptyRealm();
    }

    public void TransitionToMapEditor()
    {
        TurnManager.StopAllInputs();
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }
}
