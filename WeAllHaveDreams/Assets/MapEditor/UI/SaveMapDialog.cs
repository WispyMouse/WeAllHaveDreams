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
        SaveMapButton.interactable = false;
        gameObject.SetActive(true);
    }

    public void Save()
    {
        MapEditorFileManagement.CurrentMapName = MapNameInput.text;
        DebugTextLog.AddTextToLog($"Saving realm as ${MapNameInput.text}...", DebugTextLogChannel.MapEditorOperations);
        Task.Run(MapEditorFileManagementInstance.SaveNewRealm);
        Close();
    }
}
