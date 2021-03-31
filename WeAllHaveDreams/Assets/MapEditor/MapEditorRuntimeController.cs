using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditorRuntimeController : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public LocationInput LocationInputController;
    public MapEditorFileManagement MapEditorFileManagementInstance;
    public MapEditorRibbon MapEditorRibbonInstance;
    public MapEditorPalette MapEditorPaletteInstance;

    public PaletteSettings LeftClickPaletteSettings;
    public PaletteSettings RightClickPaletteSettings;

    List<MapEditorInput> ActionHistory { get; set; } = new List<MapEditorInput>();
    int? historyPointer { get; set; } = null;

    private void Update()
    {
        bool inputHandled = HandleClick();
        inputHandled |= HandleRedoUndo();
    }

    public void Startup()
    {
        LeftClickPaletteSettings = new TilePlacementPalette(TileLibrary.GetTile("Floor"));
        RightClickPaletteSettings = new ClearTilePalette();

        List<PaletteSettings> tileSettings = new List<PaletteSettings>();

        foreach (GameplayTile curTile in TileLibrary.GetAllTiles())
        {
            tileSettings.Add(new TilePlacementPalette(curTile));
        }

        MapEditorPaletteInstance.Open(tileSettings);
    }

    bool HandleClick()
    {
        int? click = Input.GetMouseButtonDown(0) ? 0 : Input.GetMouseButtonDown(1) ? (int?)1 : null;
        if (click.HasValue && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3Int? worldpoint = LocationInputController.GetHoveredTilePosition(false);

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

    bool HandleRedoUndo()
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

    void ApplyTilePalette(Vector3Int worldPoint, PaletteSettings toApply)
    {
        if (toApply == null)
        {
            DebugTextLog.AddTextToLog("Tried to apply a palette, but it was null.", DebugTextLogChannel.MapEditorOperations);
            return;
        }

        GameplayTile tileAtPosition = WorldContextInstance.MapHolder.GetGameplayTile(worldPoint);
        string replacedTile = null;

        if (tileAtPosition != null)
        {
            replacedTile = tileAtPosition.TileName;
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

    public void SetPalette(PaletteSettings toPalette)
    {
        LeftClickPaletteSettings = toPalette;
    }
}
