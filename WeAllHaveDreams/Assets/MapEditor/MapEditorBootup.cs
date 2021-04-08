using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootup entry point for the MapEditor.
/// This is the first logic point for the MapEditor, where all of the runtimes are initialized.
/// </summary>
public class MapEditorBootup : MapBootup
{
    public LoadMapDialog LoadMapDialogInstance;
    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;
    public MapEditorRibbon MapEditorRibbonInstance;

    protected override IEnumerator Startup()
    {
        LocationInput.SetTileCursorVisibility(true);

        DebugTextLog.AddTextToLog("Press Z to undo and Y to redo", DebugTextLogChannel.DebugOperationInputInstructions);

        LoadMapDialogInstance.Open();
        MapEditorRuntimeControllerInstance.Startup();

        yield break;
    }

    protected override IEnumerator LoadRealm()
    {
        yield return MapEditorRuntimeControllerInstance.LoadRealm(GetRealm());
    }

    protected override Realm GetRealm()
    {
        if (GameplayMapBootup.WIPRealm == null)
        {
            return Realm.GetEmptyRealm();
        }
        else
        {
            return GameplayMapBootup.WIPRealm;
        }
    }
}
