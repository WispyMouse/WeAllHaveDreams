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

    public void SetValues(PaletteOptions options, System.Action<PaletteOptions> clickCallback)
    {
        RepresentedOptions = options;

        ButtonImage.gameObject.SetActive(false);
        ButtonText.gameObject.SetActive(true);
        ButtonText.text = RepresentedOptions.OptionsName;

        ButtonBehavior.onClick.RemoveAllListeners();
        ButtonBehavior.onClick.AddListener(() => clickCallback(RepresentedOptions));
    }
}
