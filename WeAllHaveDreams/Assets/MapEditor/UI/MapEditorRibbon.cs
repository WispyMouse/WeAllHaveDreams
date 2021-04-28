﻿using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditorRibbon : MonoBehaviour
{
    public MapEditorPalette MapEditorPaletteInstance;
    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    public SaveMapDialog SaveMapDialogInstance;
    public LoadMapDialog LoadMapDialogInstance;

    public Button PlayButton;

    private void Awake()
    {
        MapEditorRuntimeControllerInstance.MapSavedEvent += MapSaved;
        MapEditorRuntimeControllerInstance.MapLoadedEvent += MapLoaded;
        MapEditorRuntimeControllerInstance.MapChangedEvent += MapChanged;
    }

    public void SaveButtonPressed()
    {
        if (GameplayMapBootup.WIPRealm != null)
        {
            StartCoroutine(QuickSave());
        }
        else
        {
            SaveMapDialogInstance.Open();
        }
    }

    public void MapLoaded(object sender, Realm realm)
    {
        PlayButton.interactable = true;
    }

    public void MapSaved(object sender, Realm realm)
    {
        PlayButton.interactable = true;
    }

    public void MapChanged(object sender, Realm realm)
    {
        PlayButton.interactable = false;
    }

    public void PlayButtonPressed()
    {
        // HACK: Counting on MapBootup.WIPRealm to be set already
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    IEnumerator QuickSave()
    {
        yield return MapEditorRuntimeControllerInstance.SaveRealm();
        DebugTextLog.AddTextToLog("Map Saved!", DebugTextLogChannel.MapEditorOperations);
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

        List<PaletteOptions> paletteOptions = new List<PaletteOptions>()
        {
            new SingleClickTilePaintOption(),
            new FloodFillTilePaintOption()
        };

        MapEditorPaletteInstance.OpenTab(new PaletteTab("Tiles", tileSettings, paletteOptions));
    }

    public void StructurePaletteClicked()
    {
        List<PaletteSettings> structureSettings = new List<PaletteSettings>();

        foreach (MapStructure structure in StructureLibrary.GetAllStructures())
        {
            structureSettings.Add(new StructurePlacementPalette(structure));
        }

        MapEditorPaletteInstance.OpenTab(new PaletteTab("Structures", structureSettings));
    }

    public void FeaturePaletteClicked()
    {
        List<PaletteSettings> featureSettings = new List<PaletteSettings>();

        foreach (MapFeature feature in FeatureLibrary.GetAllFeatures())
        {
            featureSettings.Add(new FeaturePlacementPalette(feature));
        }

        MapEditorPaletteInstance.OpenTab(new PaletteTab("Features", featureSettings));
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

        MapEditorPaletteInstance.OpenTab(new PaletteTab("Faction", ownershipSettings));
    }

    public void MobPaletteClicked()
    {
        List<PaletteSettings> mobSettings = new List<PaletteSettings>();

        foreach (MobConfiguration config in MobLibrary.GetAllMobs())
        {
            mobSettings.Add(new MobPalette(config));
        }

        MapEditorPaletteInstance.OpenTab(new PaletteTab("Mobs", mobSettings));
    }

    public void LoadMapButtonClicked()
    {
        LoadMapDialogInstance.Open();
    }

    public void NewMapButtonPressed()
    {
        MapEditorRuntimeControllerInstance.NewMap();
    }
}
