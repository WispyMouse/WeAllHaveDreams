using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorPalette : MonoBehaviour
{
    public PaletteButton PaletteButtonPF;
    public Transform PaletteItemHolder;

    public ActivePalettePanel LeftActivePalette;
    public ActivePalettePanel RightActivePalette;

    public Text CurrentSelectedPaletteTabLabel;
    public event EventHandler<string> PaletteTabChanged;

    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    List<PaletteButton> ActiveButtons { get; set; } = new List<PaletteButton>();

    private void Awake()
    {
        PaletteTabChanged += PaletteTabChangedEvent;
    }

    // TODO: Make the settings a class that holds data, rather than passing in all the arguments perhaps
    public void Open(string paletteTabName, IEnumerable<PaletteSettings> settings)
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

        LeftActivePalette.SetPalette(MapEditorRuntimeControllerInstance.LeftClickPaletteSettings);
        RightActivePalette.SetPalette(MapEditorRuntimeControllerInstance.RightClickPaletteSettings);

        PaletteTabChanged.Invoke(this, paletteTabName);
    }

    void PaletteButtonClicked(PaletteButton button)
    {
        MapEditorRuntimeControllerInstance.SetPalette(button.RepresentedOption);
        LeftActivePalette.SetPalette(MapEditorRuntimeControllerInstance.LeftClickPaletteSettings);
    }

    void PaletteTabChangedEvent(object sender, string name)
    {
        CurrentSelectedPaletteTabLabel.text = name;
    }
}
