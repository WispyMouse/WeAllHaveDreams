using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayMapBootup : MapBootup
{
    public const string MapFolderPathRelative = "Resources/Maps";
    public static string MapFolderPath
    {
        get
        {
            return Path.Combine(applicationDataPath, MapFolderPathRelative).Replace("\\", "/");
        }
    }

    static string applicationDataPath { get; set; }

    public const string MapFileSuffix = ".json";
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

    private void Awake()
    {
        applicationDataPath = Application.dataPath;
    }

    protected override IEnumerator Startup()
    {
        TurnManagerInstance.GameplayReady();

        DebugTextLog.AddTextToLog("Press M to enter Map Editor mode", DebugTextLogChannel.DebugOperationInputInstructions);
        yield break;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TransitionToMapEditor();
        }
    }

    public void TransitionToMapEditor()
    {
        TurnManager.StopAllInputs();
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }

    protected override Realm GetRealm()
    {
        if (WIPRealm != null)
        {
            return WIPRealm;
        }

        foreach (string filePath in Directory.GetFiles(MapFolderPath, MapFileSearch, SearchOption.AllDirectories))
        {
            string sanitizedFilePath = filePath.Replace("\\", "/");

            // HACK TODO: This just gets the first one it finds and calls it good
            string fileText;

            using (var reader = File.OpenText(sanitizedFilePath))
            {
                // TODO: This was async, we want to restore this to properly read async so we're not blocking threads as we read
                fileText = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Realm>(fileText);
        }

        DebugTextLog.AddTextToLog("Attempted to load default realm, but could not find any, returning empty", DebugTextLogChannel.DebugLogging);

        return Realm.GetEmptyRealm();
    }
}
