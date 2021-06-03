using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteOptionsButton : MonoBehaviour
{
    public Button ButtonBehavior;
    public Image ButtonImage;
    public Text ButtonText;

    PaletteOptions RepresentedOptions { get; set; }

    public void SetValues(PaletteOptions options, PaletteOptionsCollection selectedOptions, System.Action<PaletteOptions> clickCallback)
    {
        RepresentedOptions = options;

        ButtonImage.gameObject.SetActive(false);
        ButtonText.gameObject.SetActive(true);
        UpdateSelectedState(selectedOptions.Contains(RepresentedOptions));

        ButtonBehavior.onClick.RemoveAllListeners();
        ButtonBehavior.onClick.AddListener(() => clickCallback(RepresentedOptions));
    }

    public void SelectedOptionsUpdate(PaletteOptionsCollection newCollection)
    {
        UpdateSelectedState(newCollection.Contains(RepresentedOptions));
    }

    void UpdateSelectedState(bool selected)
    {
        if (selected)
        {
            ButtonText.text = $"*{RepresentedOptions.OptionsName}*";
        }
        else
        {
            ButtonText.text = RepresentedOptions.OptionsName;
        }
    }
}
