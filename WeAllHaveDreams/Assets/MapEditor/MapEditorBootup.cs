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
    /// <summary>
    /// Pointer to the <see cref="MapEditorRuntimeController"/> in the scene.
    /// This class hands off the start of the MapEditor scene to this after it's done loading in.
    /// </summary>
    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    /// <inheritdoc />
    protected override IEnumerator Startup()
    {
        MapEditorRuntimeControllerInstance.Startup();

        yield break;
    }

    /// <inheritdoc />
    protected override IEnumerator LoadRealm()
    {
        // Passes the load call to the runtime controller
        yield return MapEditorRuntimeControllerInstance.LoadRealm(GetRealm());
    }
    
    /// <inheritdoc />
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
