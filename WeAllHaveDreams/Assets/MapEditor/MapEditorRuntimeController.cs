using System;
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
    /// The current collection of options.
    /// These can modify PaletteSettings when applied.
    /// </summary>
    public PaletteOptionsCollection SelectedOptions { get; set; } = new PaletteOptionsCollection();

    /// <summary>
    /// The active setting for Left Click.
    /// When you left click, the <see cref="PaletteSettings.ApplyPalette(WorldContext, MapCoordinates)"/> of this setting is applied.
    /// </summary>
    public PaletteSettings LeftClickPaletteSettings;

    /// <summary>
    /// The active setting for Right Click.
    /// When you right click, the <see cref="PaletteSettings.ApplyPalette(WorldContext, MapCoordinates)"/> of this setting is applied.
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
    /// Stores what context was previously invoked.
    /// This is so that ContinueInvoke knows when not to add to the undo history.
    /// </summary>
    MapEditorInput lastProcessedInput { get; set; }

    /// <summary>
    /// Stores input information, such as where a drag motion started at.
    /// </summary>
    Dictionary<int, InputContext> currentInputContexts { get; set; } = new Dictionary<int, InputContext>();

    /// <summary>
    /// An event fired whenever a Map is saved.
    /// This applies to both newly saved Realms and updated saving ones.
    /// </summary>
    public event EventHandler<Realm> MapSavedEvent;

    /// <summary>
    /// An event fired whenever a Map is loaded.
    /// </summary>
    public event EventHandler<Realm> MapLoadedEvent;

    /// <summary>
    /// An event fired whenever a Map is changed.
    /// This includes undoing actions. It's up to the listener to determine if the map is "dirty".
    /// </summary>
    public event EventHandler<Realm> MapChangedEvent;

    /// <summary>
    /// Function that is called by <see cref="MapEditorBootup"/> when the editor's essential contexts have been loaded in.
    /// By the end of this function, the MapEditor should be ready to use.
    /// </summary>
    public void Startup()
    {
        LocationInput.SetTileCursorVisibility(true);

        DebugTextLog.AddTextToLog("Press Z to undo and Y to redo", DebugTextLogChannel.DebugOperationInputInstructions);

        LoadMapDialogInstance.Open();

        LeftClickPaletteSettings = new TilePlacementPalette(TileLibrary.GetAllTiles().First());
        RightClickPaletteSettings = new ClearTilePalette();

        SelectedOptions = new PaletteOptionsCollection();
        SelectedOptions.AddPaletteOption(new SingleClickTilePaintOption());

        MapEditorRibbonInstance.TilePaletteClicked();

        NewMap();

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
        ActionHistory.Clear();
        historyPointer = null;

        WorldContextInstance = WorldContext.GetWorldContext();
        WorldContextInstance.ClearEverything();

        DebugTextLog.AddTextToLog($"Loading realm: {toLoad.Name}, {toLoad.RealmCoordinates.Count()}", DebugTextLogChannel.DebugLogging);

        WorldContextInstance.LoadFromRealm(toLoad);
        currentMapName = toLoad.Name;
        GameplayMapBootup.WIPRealm = toLoad;

        DebugTextLog.AddTextToLog("Loaded realm", DebugTextLogChannel.DebugLogging);
        MapLoadedEvent.Invoke(this, GameplayMapBootup.WIPRealm);

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
        MapSavedEvent.Invoke(this, GameplayMapBootup.WIPRealm);
    }

    /// <summary>
    /// Sets the current working map's name.
    /// TODO: This is a temporary holding place for this data.
    /// </summary>
    /// <param name="name">The name of the map.</param>
    public void SetCurrentMapName(string name)
    {
        currentMapName = name;

        if (GameplayMapBootup.WIPRealm != null)
        {
            GameplayMapBootup.WIPRealm.Name = currentMapName;
        }
    }

    /// <summary>
    /// Sets up for a new Realm, clearing previous contexts.
    /// </summary>
    public void NewMap()
    {
        GameplayMapBootup.WIPRealm = null;
        currentMapName = null;
        WorldContextInstance.ClearEverything();
        ActionHistory.Clear();
        historyPointer = null;

        MapChangedEvent.Invoke(this, null);
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

        if (inputHandled)
        {
            MapChangedEvent.Invoke(this, GameplayMapBootup.WIPRealm);
        }
    }

    /// <summary>
    /// Handles painting Palettes by mouse click.
    /// </summary>
    /// <returns>True if input has been handled, False if nothing has been performed.</returns>
    private bool HandleClick()
    {
        bool handled = HandleMouseButton(0);
        handled = handled || HandleMouseButton(1);
        return handled;
    }

    private bool HandleMouseButton(int index)
    {
        // HACK: Should look this up based on index, rather than ternary operator
        PaletteSettings consideredSettings = index == 0 ? LeftClickPaletteSettings : RightClickPaletteSettings;
        MapCoordinates? worldpoint = LocationInput.GetHoveredTilePosition(false);

        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(index))
        {
            InputContext existingContext;
            if (currentInputContexts.TryGetValue(index, out existingContext))
            {
                DebugTextLog.AddTextToLog("There was an existing input context on inital click, but there shouldn't be.", DebugTextLogChannel.MapEditorOperations);
                return false;
            }

            // We didn't click on a position, so do nothing
            if (!worldpoint.HasValue)
            {
                return false;
            }

            InputContext newContext = new InputContext(worldpoint.Value);
            MapEditorInput consideredInput = consideredSettings.ApplyPalette(WorldContextInstance, worldpoint.Value);

            OptionPaintApplication application = SelectedOptions.DetermineApplication(WorldContextInstance, consideredInput, newContext);

            switch (application)
            {
                case OptionPaintApplication.Unmodified:
                case OptionPaintApplication.Invoke:
                case OptionPaintApplication.FinishInvoke:
                    ApplyTilePalette(worldpoint.Value, consideredSettings, newContext, application);
                    return true;
                case OptionPaintApplication.ContinueInvoke:
                    currentInputContexts.Add(index, newContext);
                    ApplyTilePalette(worldpoint.Value, consideredSettings, newContext, application);
                    return true;
                default:
                    currentInputContexts.Add(index, newContext);
                    break;
            }
        }

        if (Input.GetMouseButton(index))
        {
            InputContext existingContext;
            if (!currentInputContexts.TryGetValue(index, out existingContext))
            {
                return false;
            }

            // We haven't actually moved the cursor, so don't consider invoking
            if (existingContext.CurrentPosition == worldpoint.Value)
            {
                return false;
            }

            existingContext.CurrentPosition = worldpoint.Value;
            MapEditorInput consideredInput = consideredSettings.ApplyPalette(WorldContextInstance, worldpoint.Value);
            OptionPaintApplication application = SelectedOptions.DetermineApplication(WorldContextInstance, consideredInput, existingContext);

            switch (application)
            {
                case OptionPaintApplication.Invoke:
                case OptionPaintApplication.FinishInvoke:
                    existingContext.EndClick = worldpoint.Value;
                    currentInputContexts.Remove(index);
                    ApplyTilePalette(worldpoint.Value, consideredSettings, existingContext, application);
                    return true;
                case OptionPaintApplication.ContinueInvoke:
                    ApplyTilePalette(worldpoint.Value, consideredSettings, existingContext, application);
                    return true;
                default:
                    return false;
            }
        }

        if (Input.GetMouseButtonUp(index))
        {
            InputContext existingContext;
            if (!currentInputContexts.TryGetValue(index, out existingContext))
            {
                return false;
            }

            currentInputContexts.Remove(index);

            // We aren't somewhere we can determine, so we can't take any operations
            if (!worldpoint.HasValue)
            {
                return false;
            }

            existingContext.EndClick = worldpoint.Value;
            MapEditorInput consideredInput = consideredSettings.ApplyPalette(WorldContextInstance, worldpoint.Value);
            OptionPaintApplication application = SelectedOptions.DetermineApplication(WorldContextInstance, consideredInput, existingContext);

            switch (application)
            {
                case OptionPaintApplication.FinishInvoke:
                case OptionPaintApplication.Invoke:
                    ApplyTilePalette(worldpoint.Value, consideredSettings, existingContext, application);
                    return true;
                default:
                    return false;
            }
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
    /// <param name="toApply">The PaletteSettings to pull from. The <see cref="MapEditorInput"/> retried from <see cref="PaletteSettings.ApplyPalette(WorldContext, MapCoordinates)"/> will be used.</param>
    void ApplyTilePalette(MapCoordinates worldPoint, PaletteSettings toApply, InputContext inputContext, OptionPaintApplication invokeContext)
    {
        if (toApply == null)
        {
            DebugTextLog.AddTextToLog("Tried to apply a palette, but it was null.", DebugTextLogChannel.MapEditorOperations);
            return;
        }

        // If we're meant to continue the previous invoke, and there isn't a context, set it and flag that we've set it
        bool setContext = false;
        MapEditorInput toInvoke;

        if (invokeContext == OptionPaintApplication.ContinueInvoke && lastProcessedInput != null)
        {
            toInvoke = lastProcessedInput;
        }
        else if (invokeContext == OptionPaintApplication.FinishInvoke && lastProcessedInput != null)
        {
            toInvoke = lastProcessedInput;
        }
        else
        {
            toInvoke = toApply.ApplyPalette(WorldContextInstance, worldPoint);
            setContext = true;
        }

        SelectedOptions.ApplyOptions(WorldContextInstance, toInvoke, inputContext);
        toInvoke.Invoke(WorldContextInstance);

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

        if (setContext)
        {
            ActionHistory.Add(toInvoke);
        }

        historyPointer = null;
        MapEditorRibbonInstance.MapMarkedAsDirty();

        if (invokeContext == OptionPaintApplication.ContinueInvoke)
        {
            lastProcessedInput = toInvoke;
        }
        else
        {
            lastProcessedInput = null;
        }
    }
}
