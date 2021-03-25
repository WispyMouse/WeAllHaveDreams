﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorPalette : MonoBehaviour
{
    public PaletteButton PaletteButtonPF;
    public Transform PaletteItemHolder;

    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    List<PaletteButton> ActiveButtons { get; set; } = new List<PaletteButton>();

    public void Open(IEnumerable<PaletteSettings> settings)
    {
        foreach (PaletteButton curButton in ActiveButtons)
        {
            Destroy(curButton.gameObject);
        }

        ActiveButtons = new List<PaletteButton>();

        foreach (PaletteSettings curSetting in settings)
        {
            PaletteButton newButton = Instantiate(PaletteButtonPF, PaletteItemHolder);
            newButton.SetTile(curSetting, PaletteButtonClicked);
            ActiveButtons.Add(newButton);
        }
    }

    void PaletteButtonClicked(PaletteButton button)
    {
        MapEditorRuntimeControllerInstance.SetPalette(button.RepresentedOption);
    }
}
