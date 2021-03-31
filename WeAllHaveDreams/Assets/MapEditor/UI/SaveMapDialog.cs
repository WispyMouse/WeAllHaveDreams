using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SaveMapDialog : MonoBehaviour
{
    public Button SaveMapButton;
    public InputField MapNameInput;

    public MapEditorFileManagement MapEditorFileManagementInstance;
    public MapEditorRibbon MapEditorRibbonInstance;

    IEnumerator CurrentSavingRoutine { get; set; }

    public void NameChanged()
    {
        SaveMapButton.interactable = !string.IsNullOrWhiteSpace(MapNameInput.text);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        SaveMapButton.interactable = !string.IsNullOrWhiteSpace(MapNameInput.text);
        gameObject.SetActive(true);
    }

    public void Save()
    {
        StartCoroutine(ProcessSave());
    }

    IEnumerator ProcessSave()
    {
        SaveMapButton.interactable = false;
        MapEditorFileManagement.CurrentMapName = MapNameInput.text;
        DebugTextLog.AddTextToLog($"Saving realm as {MapNameInput.text}...", DebugTextLogChannel.MapEditorOperations);
        Task saveTask = Task.Run(MapEditorFileManagementInstance.SaveNewRealm);

        while (!saveTask.IsCompleted)
        {
            yield return new WaitForEndOfFrame();
        }

        DebugTextLog.AddTextToLog("Saved!", DebugTextLogChannel.MapEditorOperations);
        MapEditorRibbonInstance.MapSaved();
        Close();
    }
}
