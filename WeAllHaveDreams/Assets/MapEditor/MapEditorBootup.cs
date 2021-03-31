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
    public MapEditorRibbon MapEditorRibbonInstance;

    private void Start()
    {
        StartCoroutine(StartInternal());
    }

    IEnumerator StartInternal()
    {
        DebugTextLog.AddTextToLog("Loading Library");
        yield return ThreadDoctor.YieldAsyncOperation(SceneManager.LoadSceneAsync("Library", LoadSceneMode.Additive));

        DebugTextLog.AddTextToLog("Loading WorldContext");
        yield return ThreadDoctor.YieldAsyncOperation(SceneManager.LoadSceneAsync("WorldContext", LoadSceneMode.Additive));

        yield return ThreadDoctor.YieldTask(ConfigurationLoadingEntrypoint.LoadAllConfigurationData());

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
        MapBootup.WIPRealm = toLoad;
        MapEditorRibbonInstance.MapLoaded();
        DebugTextLog.AddTextToLog("Loaded realm", DebugTextLogChannel.DebugLogging);
    }
}
