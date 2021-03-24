using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteButton : MonoBehaviour
{
    public Button ButtonBehavior;
    public Image ButtonImage;
    public Text ButtonText;

    public GameplayTile RepresentedOption;

    public void SetTile(GameplayTile tile, System.Action<PaletteButton> clickCallback)
    {
        RepresentedOption = tile;

        ButtonImage.sprite = tile.DefaultSprite;
        ButtonImage.gameObject.SetActive(true);
        ButtonText.gameObject.SetActive(false);

        ButtonBehavior.onClick.AddListener(() => clickCallback(this));
    }
}
