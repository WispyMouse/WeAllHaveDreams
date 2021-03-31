using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditorRibbon : MonoBehaviour
{
    public MapEditorFileManagement MapEditorFileManagementInstance;

    public MapEditorPalette MapEditorPaletteInstance;

    public SaveMapDialog SaveMapDialogInstance;
    public LoadMapDialog LoadMapDialogInstance;

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

    public void MapLoaded()
    {
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

    public void TilePaletteClicked()
    {
        List<PaletteSettings> tileSettings = new List<PaletteSettings>();

        foreach (GameplayTile curTile in TileLibrary.GetAllTiles())
        {
            tileSettings.Add(new TilePlacementPalette(curTile));
        }

        MapEditorPaletteInstance.Open(tileSettings);
    }

    public void StructurePaletteClicked()
    {
        List<PaletteSettings> structureSettings = new List<PaletteSettings>();

        foreach (MapStructure structure in StructureLibrary.GetAllStructures())
        {
            structureSettings.Add(new StructurePlacementPalette(structure));
        }

        MapEditorPaletteInstance.Open(structureSettings);
    }

    public void OwnershipPaletteClicked()
    {
        List<PaletteSettings> ownershipSettings = new List<PaletteSettings>();
        ownershipSettings.Add(new OwnershipPalette(null));

        // needs a way to add more, later; find a healthy balance between "enables things" and "reasonable for usual use"
        for (int ii = 0; ii < 2; ii++)
        {
            ownershipSettings.Add(new OwnershipPalette(ii));
        }

        MapEditorPaletteInstance.Open(ownershipSettings);
    }

    public void LoadMapButtonClicked()
    {
        LoadMapDialogInstance.Open();
    }
}
