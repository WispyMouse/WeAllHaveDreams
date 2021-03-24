using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorPalette : MonoBehaviour
{
    public PaletteButton PaletteButtonPF;
    public Transform PaletteItemHolder;

    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    List<PaletteButton> ActiveButtons { get; set; } = new List<PaletteButton>();

    public void Open()
    {
        foreach (GameplayTile curTile in TileLibrary.GetAllTiles())
        {
            PaletteButton newButton = Instantiate(PaletteButtonPF, PaletteItemHolder);
            newButton.SetTile(curTile, PaletteButtonClicked);
            ActiveButtons.Add(newButton);
        }
    }

    void PaletteButtonClicked(PaletteButton button)
    {
        MapEditorRuntimeControllerInstance.SetPalette(button.RepresentedOption);
    }
}
