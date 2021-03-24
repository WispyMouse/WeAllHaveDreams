using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditorBootup : MonoBehaviour
{
    public LocationInput LocationInputInstance;
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();
    public LoadMapDialog LoadMapDialogInstance;
    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    private void Start()
    {
        StartCoroutine(StartInternal());
    }

    IEnumerator StartInternal()
    {
        DebugTextLog.AddTextToLog("Loading Library");
        AsyncOperation libraryHandle = SceneManager.LoadSceneAsync("Library", LoadSceneMode.Additive);
        while (!libraryHandle.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        DebugTextLog.AddTextToLog("Loading WorldContext");
        AsyncOperation contextHandle = SceneManager.LoadSceneAsync("WorldContext", LoadSceneMode.Additive);
        while (!contextHandle.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        Task configurationLoading = Task.Run(async() => await ConfigurationLoadingEntrypoint.LoadAllConfigurationData());
        while (!configurationLoading.IsCompleted)
        {
            yield return new WaitForEndOfFrame();
        }

        if (MapBootup.WIPRealm == null)
        {
            WorldContextInstance.MapHolder.LoadEmptyRealm();
        }
        else
        {
            WorldContextInstance.MapHolder.LoadFromRealm(MapBootup.WIPRealm);
        }

        LocationInputInstance.SetTileCursorVisibility(true);

        DebugTextLog.AddTextToLog("Press Z to undo and Y to redo", DebugTextLogChannel.DebugOperationInputInstructions);

        LoadMapDialogInstance.Open();
        MapEditorRuntimeControllerInstance.Startup();
    }

    public IEnumerator LoadRealm(Realm toLoad)
    {
        DebugTextLog.AddTextToLog($"Loading realm: {toLoad.Name}, {toLoad.RealmCoordinates.Count()}", DebugTextLogChannel.DebugLogging);
        yield return WorldContextInstance.MapHolder.LoadFromRealm(toLoad);
        DebugTextLog.AddTextToLog("Loaded realm", DebugTextLogChannel.DebugLogging);
    }
}
