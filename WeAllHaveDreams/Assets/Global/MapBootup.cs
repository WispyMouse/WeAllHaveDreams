using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Parent class for all scene initializing scripts.
/// This abstract class handles essential scene additive loading and provides a template for map initialization.
/// It is expected that after the initial bootup this class does not perform further actions.
/// </summary>
public abstract class MapBootup : MonoBehaviour
{
    /// <summary>
    /// A pointer to the existing <see cref="WorldContext"/>.
    /// This class's primary functions run before the WorldContext Singleton is set. Its value is pulled after the WorldContext scene is loaded in.
    /// </summary>
    protected WorldContext WorldContextInstance { get; set; }

    /// <summary>
    /// Unity provided Start function signal. This being here in the parent class means that children classes should not implement Start.
    /// Starts a Coroutine to switch to Coroutine style context.
    /// </summary>
    public void Start()
    {
        StartCoroutine(HandleLoading());
    }

    /// <summary>
    /// Private function for running through all of the loading steps.
    /// </summary>
    /// <returns>Yieldable IEnumerator.</returns>
    private IEnumerator HandleLoading()
    {
        yield return LoadScenesAndConfigurations();
        yield return LoadRealm();
        yield return Startup();
    }

    /// <summary>
    /// Private function for loading the essential additive scenes.
    /// Every scene that has a map requires at least these scenes to be loaded.
    /// </summary>
    /// <returns>Yieldable IEnumerator.<returns>
    private IEnumerator LoadScenesAndConfigurations()
    {
        DebugTextLog.AddTextToLog("Loading Library");
        yield return ThreadDoctor.YieldAsyncOperation(SceneManager.LoadSceneAsync("Library", LoadSceneMode.Additive));

        yield return ThreadDoctor.YieldTask(ConfigurationLoadingEntrypoint.LoadAllConfigurationData());

        DebugTextLog.AddTextToLog("Loading WorldContext");
        yield return ThreadDoctor.YieldAsyncOperation(SceneManager.LoadSceneAsync("WorldContext", LoadSceneMode.Additive));

        WorldContextInstance = WorldContext.GetWorldContext();

        DebugTextLog.AddTextToLog("Loading Camera");
        yield return ThreadDoctor.YieldAsyncOperation(SceneManager.LoadSceneAsync("Camera", LoadSceneMode.Additive));
    }

    /// <summary>
    /// Protected virtual function for loading and deploying a realm.
    /// It is expected that this will pull the Realm to load, and to load it in to the WorldContext.
    /// You may override this to make more complex map loading, otherwise it'll take the result of GetRealm and load that.
    /// </summary>
    /// <returns>Yieldable IEnumerator.</returns>
    protected virtual IEnumerator LoadRealm()
    {
        Realm usedRealm = GetRealm();

        WorldContextInstance.LoadFromRealm(usedRealm);
        yield break;
    }

    /// <summary>
    /// Protected abstract function for start up specific to this class.
    /// This is abstract to show that this template class doesn't do "enough" work to be a standalone scene.
    /// No particular work is expected, though the runtime for this scene should be activated by the end of this function.
    /// </summary>
    /// <returns>Yieldable IEnumerator.</returns>
    protected abstract IEnumerator Startup();

    /// <summary>
    /// Fetches a realm to load in to the map.
    /// This is used by <see cref="LoadRealm()"/> by default.
    /// </summary>
    /// <returns>A Realm to load. By default, it is an empty Realm. It should return what Realm will be loaded in this context.</returns>
    protected virtual Realm GetRealm()
    {
        return Realm.GetEmptyRealm();
    }
}
