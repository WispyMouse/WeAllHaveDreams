using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Main hub for MapEditor operations.
/// This handles the main dynamics of <see cref="PaletteSettings"/> being applied as <see cref="MapEditorInput"/>s.
/// Loading the realm also runs through this class.
/// </summary>
public class MapEditorRuntimeController : MonoBehaviour
{
    /// <summary>
    /// Pointer to the current WorldContext.
    /// </summary>
    private WorldContext WorldContextInstance { get; set; }

    /// <summary>
    /// Pointer to the MapEditorRibbon instance.
    /// Used to call MapEditorRibbon functions, like Load.
    /// TODO: This should point to a class that has the *features* of MapEditorRibbon, rather than pointing to the UX handler.
    /// </summary>
    public MapEditorRibbon MapEditorRibbonInstance;

    /// <summary>
    /// Pointer to the MapEditorPalette instance.
    /// This class handles Palette setting and displaying, and passes through to MapEditorPalette often.
    /// </summary>
    public MapEditorPalette MapEditorPaletteInstance;

    /// <summary>
    /// Pointer to the LoadMapDialog instance.
    /// This is opened when the scene is opened.
    /// </summary>
    public LoadMapDialog LoadMapDialogInstance;

    /// <summary>
    /// The active setting for Left Click.
    /// When you left click, the <see cref="PaletteSettings.ApplyPalette(WorldContext, Vector3Int)"/> of this setting is applied.
    /// </summary>
    public PaletteSettings LeftClickPaletteSettings;

    /// <summary>
    /// The active setting for Right Click.
    /// When you right click, the <see cref="PaletteSettings.ApplyPalette(WorldContext, Vector3Int)"/> of this setting is applied.
    /// </summary>
    public PaletteSettings RightClickPaletteSettings;

    /// <summary>
    /// History of all undo-able/redo-able inputs in the current historical context.
    /// There are situations where this history is cleared or trimmed.
    /// </summary>
    List<MapEditorInput> ActionHistory { get; set; } = new List<MapEditorInput>();

    /// <summary>
    /// Where in the <see cref="ActionHistory"/> is the "present".
    /// If this has no value, then it's assumed we're at the most recent element in ActionHistory.
    /// This is set to null when an action is taken, and everything after this pointer is cleared out of ActionHistory.
    /// </summary>
    int? historyPointer { get; set; } = null;

    /// <summary>
    /// Indicates if <see cref="Startup()"/> has been completed.
    /// </summary>
    bool StartupComplete { get; set; } = false;

    /// <summary>
    /// The name of the map currently being worked on.
    /// TODO: This is used to store data temporarily. We'll want to push this off somewhere more appropriate later.
    /// </summary>
    string currentMapName { get; set; }

    /// <summary>
    /// Function that is called by <see cref="MapEditorBootup"/> when the editor's essential contexts have been loaded in.
    /// By the end of this function, the MapEditor should be ready to use.
    /// </summary>
    public void Startup()
    {
        LocationInput.SetTileCursorVisibility(true);

        DebugTextLog.AddTextToLog("Press Z to undo and Y to redo", DebugTextLogChannel.DebugOperationInputInstructions);

        LoadMapDialogInstance.Open();

        LeftClickPaletteSettings = new TilePlacementPalette(TileLibrary.GetTile("Floor"));
        RightClickPaletteSettings = new ClearTilePalette();

        MapEditorRibbonInstance.TilePaletteClicked();

        StartupComplete = true;
    }

    /// <summary>
    /// Sets <see cref="LeftClickPaletteSettings"/> to <paramref name="toPalette"/>.
    /// TODO: This should be able to set <see cref="RightClickPaletteSettings"/> as well.
    /// </summary>
    /// <param name="toPalette"></param>
    public void SetPalette(PaletteSettings toPalette)
    {
        LeftClickPaletteSettings = toPalette;
    }

    /// <summary>
    /// Loads in the provided Realm.
    /// This clears the existing context, loads in the new one, sets the current work in progress realm, and clears the MapEditorRibbon dirty flags.
    /// </summary>
    /// <param name="toLoad"></param>
    /// <returns></returns>
    public IEnumerator LoadRealm(Realm toLoad)
    {
        WorldContextInstance = WorldContext.GetWorldContext();
        WorldContextInstance.ClearEverything();

        DebugTextLog.AddTextToLog($"Loading realm: {toLoad.Name}, {toLoad.RealmCoordinates.Count()}", DebugTextLogChannel.DebugLogging);

        WorldContextInstance.LoadFromRealm(toLoad);
        GameplayMapBootup.WIPRealm = toLoad;

        // TODO: This should be raised by an event, rather than explicitly in here.
        MapEditorRibbonInstance.MapLoaded();

        DebugTextLog.AddTextToLog("Loaded realm", DebugTextLogChannel.DebugLogging);

        yield break;
    }

    /// <summary>
    /// Saves the current Realm.
    /// This also "boxes up" the current Realm in to a saveable form.
    /// </summary>
    /// <returns>Yieldable IEnumerator.</returns>
    public IEnumerator SaveRealm()
    {
        // Stores the current map in a WIP place.
        // TODO: Put this somewhere more stable and appropriate!
        GameplayMapBootup.WIPRealm = WorldContextInstance.GenerateRealm();
        GameplayMapBootup.WIPRealm.Name = currentMapName;
        yield return FileManagement.SaveRealm(GameplayMapBootup.WIPRealm);
    }

    /// <summary>
    /// Sets the current working map's name
    /// </summary>
    /// <param name="name"></param>
    public void SetCurrentMapName(string name)
    {
        currentMapName = name;
    }

    /// <summary>
    /// Unity provided Update function, called once per frame.
    /// UX is processed here, such as painting or redo-ing.
    /// Early exits <see cref="StartupComplete"/> is false.
    /// </summary>
    private void Update()
    {
        if (!StartupComplete)
        {
            return;
        }

        bool inputHandled = HandleClick();
        inputHandled |= HandleRedoUndo();
    }

    /// <summary>
    /// Handles painting Palettes by mouse click.
    /// </summary>
    /// <returns>True if input has been handled, False if nothing has been performed.</returns>
    private bool HandleClick()
    {
        int? click = Input.GetMouseButtonDown(0) ? 0 : Input.GetMouseButtonDown(1) ? (int?)1 : null;
        if (click.HasValue && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3Int? worldpoint = LocationInput.GetHoveredTilePosition(false);

            // We didn't click on a position, so do nothing
            if (!worldpoint.HasValue)
            {
                return false;
            }

            if (click == 0)
            {
                ApplyTilePalette(worldpoint.Value, LeftClickPaletteSettings);
            }
            else
            {
                ApplyTilePalette(worldpoint.Value, RightClickPaletteSettings);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Handles redo and undo operations.
    /// </summary>
    /// <returns>True if input has been handled, False if nothing has been performed.</returns>
    private bool HandleRedoUndo()
    {
        // Z is undo
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // If HistoryPointer doesn't have a value yet, we're at the frontier
            if (!historyPointer.HasValue)
            {
                historyPointer = ActionHistory.Count - 1;
            }

            // We can't undo anything if we've already undone everything
            if (historyPointer.Value < 0)
            {
                DebugTextLog.AddTextToLog("Tried to undo, but already at the end of the history stack.", DebugTextLogChannel.DebugLogging);
                return false;
            }

            MapEditorInput replacementAction = ActionHistory[historyPointer.Value];
            replacementAction.Undo(WorldContextInstance);
            historyPointer--;

            return true;
        }

        // Y is redo
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // If HistoryPointer doesn't have a value yet, we're at the frontier; in this case, it means we can't redo anything
            if (!historyPointer.HasValue)
            {
                return false;
            }

            // Likewise, can't redo if we're already at the front
            if ((historyPointer.Value + 1) == ActionHistory.Count)
            {
                return false;
            }

            historyPointer++;
            MapEditorInput replacementAction = ActionHistory[historyPointer.Value];
            replacementAction.Invoke(WorldContextInstance);

            // If we've caught up, we don't need to track our pointer anymore
            if (historyPointer > ActionHistory.Count)
            {
                historyPointer = null;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Applies <paramref name="toApply"/> to <paramref name="worldPoint"/> on the Map.
    /// </summary>
    /// <param name="worldPoint">Position to paint on.</param>
    /// <param name="toApply">The PaletteSettings to pull from. The <see cref="MapEditorInput"/> retried from <see cref="PaletteSettings.ApplyPalette(WorldContext, Vector3Int)"/> will be used.</param>
    void ApplyTilePalette(Vector3Int worldPoint, PaletteSettings toApply)
    {
        if (toApply == null)
        {
            DebugTextLog.AddTextToLog("Tried to apply a palette, but it was null.", DebugTextLogChannel.MapEditorOperations);
            return;
        }

        MapEditorInput input = toApply.ApplyPalette(WorldContextInstance, worldPoint);
        input.Invoke(WorldContextInstance);

        if (historyPointer.HasValue)
        {
            if (historyPointer.Value < 0)
            {
                ActionHistory = new List<MapEditorInput>();
            }
            else
            {
                ActionHistory.RemoveRange(historyPointer.Value, ActionHistory.Count - historyPointer.Value);
            }
        }

        ActionHistory.Add(input);
        historyPointer = null;
        MapEditorRibbonInstance.MapMarkedAsDirty();
    }
}
