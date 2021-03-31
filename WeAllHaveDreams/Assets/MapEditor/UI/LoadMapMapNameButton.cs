using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMapMapNameButton : MonoBehaviour
{
    public Text MapNameLabel;

    Realm representedRealm { get; set; }
    Action<Realm> ClickedCallback { get; set; }

    public void SetRealm(Realm realm, Action<Realm> clickCallback)
    {
        representedRealm = realm;
        MapNameLabel.text = realm.Name;
        ClickedCallback = clickCallback;
    }

    public void ButtonClicked()
    {
        ClickedCallback.Invoke(representedRealm);
    }
}
