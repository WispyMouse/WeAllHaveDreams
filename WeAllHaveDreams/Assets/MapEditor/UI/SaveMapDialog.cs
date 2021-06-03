using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SaveMapDialog : MonoBehaviour
{
    public Button SaveMapButton;
    public InputField MapNameInput;

    public MapEditorRibbon MapEditorRibbonInstance;
    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

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
        MapEditorRuntimeControllerInstance.SetCurrentMapName(MapNameInput.text);
        DebugTextLog.AddTextToLog($"Saving realm as {MapNameInput.text}...", DebugTextLogChannel.MapEditorOperations);
        yield return MapEditorRuntimeControllerInstance.SaveRealm();

        DebugTextLog.AddTextToLog("Saved!", DebugTextLogChannel.MapEditorOperations);
        Close();
    }
}
