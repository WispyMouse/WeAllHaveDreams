using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditorRibbon : MonoBehaviour
{
    public MapEditorFileManagement MapEditorFileManagementInstance;

    public SaveMapDialog SaveMapDialogInstance;
    public Button PlayButton;

    public void SaveButtonPressed()
    {
        if (MapEditorFileManagementInstance.MapHasBeenSavedBefore)
        {
            StartCoroutine(QuickSave());
        }
        else
        {
            SaveMapDialogInstance.Open();
        }
    }

    async Task SaveExistingRealm()
    {
        await MapEditorFileManagementInstance.SaveExistingRealm();
        DebugTextLog.AddTextToLog("Map Saved!", DebugTextLogChannel.MapEditorOperations);
        PlayButton.interactable = true;
    }

    public void PlayButtonPressed()
    {
        // HACK: Counting on MapBootup.WIPRealm to be set already
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    IEnumerator QuickSave()
    {
        Task saveTask = Task.Run(SaveExistingRealm);
        while (!saveTask.IsCompleted)
        {
            yield return new WaitForEndOfFrame();
        }
        MapSaved();
    }

    public void MapSaved()
    {
        PlayButton.interactable = true;
    }

    public void MapMarkedAsDirty()
    {
        PlayButton.interactable = false;
    }
}
