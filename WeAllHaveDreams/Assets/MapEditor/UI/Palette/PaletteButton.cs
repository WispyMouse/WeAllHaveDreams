using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteButton : MonoBehaviour
{
    public Button ButtonBehavior;
    public Image ButtonImage;
    public Text ButtonText;

    public PaletteSettings RepresentedOption;

    public void SetTile(PaletteSettings settings, System.Action<PaletteButton> clickCallback)
    {
        RepresentedOption = settings;

        Sprite buttonImage = settings.GetButtonSprite();

        if (buttonImage != null)
        {
            ButtonImage.sprite = buttonImage;
            ButtonImage.gameObject.SetActive(true);
            ButtonText.gameObject.SetActive(false);
        }
        else
        {
            ButtonText.text = settings.GetButtonLabel();
            ButtonImage.gameObject.SetActive(false);
            ButtonText.gameObject.SetActive(true);
        }

        ButtonBehavior.onClick.AddListener(() => clickCallback(this));
    }
}
