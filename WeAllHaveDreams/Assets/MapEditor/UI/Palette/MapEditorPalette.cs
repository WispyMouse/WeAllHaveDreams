using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorPalette : MonoBehaviour
{
    public PaletteSettingsButton PaletteSettingsButtonPF;
    public Transform PaletteSettingsItemHolder;

    public PaletteOptionsButton PaletteOptionsButtonPF;
    public Transform PaletteOptionsItemHolder;

    public ActivePalettePanel LeftActivePalette;
    public ActivePalettePanel RightActivePalette;

    public Text CurrentSelectedPaletteTabLabel;
    public event EventHandler<PaletteTab> PaletteTabChanged;

    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    List<PaletteSettingsButton> ActiveSettingsButtons { get; set; } = new List<PaletteSettingsButton>();
    List<PaletteOptionsButton> ActiveOptionsButtons { get; set; } = new List<PaletteOptionsButton>();

    private void Awake()
    {
        PaletteTabChanged += PaletteTabChangedEvent;
    }

    public void OpenTab(PaletteTab toOpen)
    {
        PaletteTabChanged.Invoke(this, toOpen);
    }

    void PaletteSettingsButtonClicked(PaletteSettings pressed)
    {
        MapEditorRuntimeControllerInstance.SetPalette(pressed);
        LeftActivePalette.SetPalette(MapEditorRuntimeControllerInstance.LeftClickPaletteSettings);
    }

    void PaletteOptionsButtonClicked(PaletteOptions pressed)
    {
        MapEditorRuntimeControllerInstance.SelectedOptions.AddPaletteOption(pressed);

        foreach (PaletteOptionsButton button in ActiveOptionsButtons)
        {
            button.SelectedOptionsUpdate(MapEditorRuntimeControllerInstance.SelectedOptions);
        }
    }

    void PaletteTabChangedEvent(object sender, PaletteTab toOpen)
    {
        CurrentSelectedPaletteTabLabel.text = toOpen.TabName;

        foreach (PaletteSettingsButton curButton in ActiveSettingsButtons)
        {
            Destroy(curButton.gameObject);
        }

        ActiveSettingsButtons = new List<PaletteSettingsButton>();

        if (!toOpen.Settings.Any())
        {
            PaletteOptionsItemHolder.gameObject.SetActive(false);
        }
        else
        {
            PaletteOptionsItemHolder.gameObject.SetActive(true);

            foreach (PaletteSettings curSetting in toOpen.Settings)
            {
                PaletteSettingsButton newButton = Instantiate(PaletteSettingsButtonPF, PaletteSettingsItemHolder);
                newButton.SetValues(curSetting, PaletteSettingsButtonClicked);
                ActiveSettingsButtons.Add(newButton);
            }
        }

        foreach (PaletteOptionsButton curButton in ActiveOptionsButtons)
        {
            Destroy(curButton.gameObject);
        }

        ActiveOptionsButtons = new List<PaletteOptionsButton>();

        if (!toOpen.Options.Any())
        {
            PaletteOptionsItemHolder.gameObject.SetActive(false);
        }
        else
        {
            PaletteOptionsItemHolder.gameObject.SetActive(true);

            foreach (PaletteOptions curOptions in toOpen.Options)
            {
                PaletteOptionsButton newButton = Instantiate(PaletteOptionsButtonPF, PaletteOptionsItemHolder);
                newButton.SetValues(curOptions, MapEditorRuntimeControllerInstance.SelectedOptions, PaletteOptionsButtonClicked);
                ActiveOptionsButtons.Add(newButton);
            }
        }

        LeftActivePalette.SetPalette(MapEditorRuntimeControllerInstance.LeftClickPaletteSettings);
        RightActivePalette.SetPalette(MapEditorRuntimeControllerInstance.RightClickPaletteSettings);
    }
}
