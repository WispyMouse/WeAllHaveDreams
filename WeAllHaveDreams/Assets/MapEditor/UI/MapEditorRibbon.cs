using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MapEditorRibbon : MonoBehaviour
{
    public MapEditorFileManagement MapEditorFileManagementInstance;

    public SaveMapDialog SaveMapDialogInstance;

    public void SaveButton()
    {
        if (MapEditorFileManagementInstance.MapHasBeenSavedBefore)
        {
            Task.Run(SaveExistingRealm);
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
    }
}
