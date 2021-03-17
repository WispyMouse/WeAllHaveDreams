using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditorBootup : MonoBehaviour
{
    public LocationInput LocationInputInstance;
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

        if (MapBootup.WIPRealm == null)
        {
            await WorldContextInstance.MapHolder.LoadEmptyRealm();
        }
        else
        {
            await WorldContextInstance.MapHolder.LoadFromRealm(MapBootup.WIPRealm);
        }

        LocationInputInstance.SetTileCursorVisibility(true);

        DebugTextLog.AddTextToLog("Press Z to undo and Y to redo", DebugTextLogChannel.DebugOperationInputInstructions);
    }
}
