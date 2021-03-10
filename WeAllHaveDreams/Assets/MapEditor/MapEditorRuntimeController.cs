using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorRuntimeController : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public LocationInput LocationInputController;
    List<TileReplacementAction> ActionHistory { get; set; } = new List<TileReplacementAction>();
    int? historyPointer { get; set; } = null;

    private void Update()
    {
        bool inputHandled = HandleClick();
        inputHandled |= HandleRedoUndo();
    }

    bool HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int? worldpoint = LocationInputController.GetHoveredTilePosition(false);

            // We didn't click on a position, so do nothing
            if (!worldpoint.HasValue)
            {
                return false;
            }

            ApplyTilePalette(worldpoint.Value);
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

            TileReplacementAction replacementAction = ActionHistory[historyPointer.Value];
            WorldContextInstance.MapHolder.SetTile(replacementAction.Position, TileLibrary.GetTile(replacementAction.Removed));
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
            if (historyPointer.Value == ActionHistory.Count)
            {
                return false;
            }

            historyPointer++;
            TileReplacementAction replacementAction = ActionHistory[historyPointer.Value];
            WorldContextInstance.MapHolder.SetTile(replacementAction.Position, TileLibrary.GetTile(replacementAction.Added));

            // If we've caught up, we don't need to track our pointer anymore
            if (historyPointer > ActionHistory.Count)
            {
                historyPointer = null;
            }

            return true;
        }

        return false;
    }

    void ApplyTilePalette(Vector3Int worldPoint)
    {
        GameplayTile tileAtPosition = WorldContextInstance.MapHolder.GetGameplayTile(worldPoint);
        string replacedTile = null;

        if (tileAtPosition != null)
        {
            replacedTile = tileAtPosition.name;
        }

        TileReplacementAction replacementAction = new TileReplacementAction(worldPoint, replacedTile, "Floor");
        WorldContextInstance.MapHolder.SetTile(worldPoint, TileLibrary.GetTile("Floor"));

        if (historyPointer.HasValue)
        {
            ActionHistory.RemoveRange(historyPointer.Value, ActionHistory.Count - historyPointer.Value);
        }

        ActionHistory.Add(replacementAction);
        historyPointer = null;
    }
}
