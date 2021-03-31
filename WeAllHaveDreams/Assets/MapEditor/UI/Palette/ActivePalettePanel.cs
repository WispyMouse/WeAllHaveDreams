using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePalettePanel : MonoBehaviour
{
    public Text Title;
    public Image Icon;

    public Sprite DefaultIcon;

    public void SetPalette(PaletteSettings forSetting)
    {
        Title.text = forSetting.GetButtonLabel();

        Sprite icon = forSetting.GetButtonSprite();
        if (icon != null)
        {
            Icon.sprite = icon;
        }
        else
        {
            Icon.sprite = DefaultIcon;
        }
    }
}
