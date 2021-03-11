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

        DebugTextLog.AddTextToLog("Press P to enter Play mode", DebugTextLogChannel.DebugOperationInputInstructions);
        DebugTextLog.AddTextToLog("Press Z to undo and Y to redo", DebugTextLogChannel.DebugOperationInputInstructions);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TransitionToGameplay();
        }
    }

    public void TransitionToGameplay()
    {
        DebugTextLog.AddTextToLog("Saving map to transition to Gameplay Scene", DebugTextLogChannel.DebugOperations);

        Realm savedRealm = PackUpAndSaveRealm();
        MapBootup.WIPRealm = savedRealm;

        DebugTextLog.AddTextToLog("Save complete, assigned to WIPRealm", DebugTextLogChannel.DebugOperations);

        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    Realm PackUpAndSaveRealm()
    {
        Realm newRealm = new Realm();

        List<RealmCoordinate> realmCoordinates = new List<RealmCoordinate>();

        foreach (Vector3Int position in WorldContextInstance.MapHolder.GetAllTiles())
        {
            GameplayTile tile = WorldContextInstance.MapHolder.GetGameplayTile(position);
            realmCoordinates.Add(new RealmCoordinate() { Position = position, Tile = tile.name });
        }

        newRealm.RealmCoordinates = realmCoordinates;

        return newRealm;
    }
}
