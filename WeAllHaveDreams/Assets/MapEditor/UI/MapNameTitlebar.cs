using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNameTitlebar : MonoBehaviour
{
    const string UnnamedDefault = "(unsaved map)";
    public Text MapNameLabel;

    public MapEditorRuntimeController MapEditorRuntimeControllerInstance;

    private void Awake()
    {
        MapNameLabel.text = UnnamedDefault;
        MapNameLabel.fontStyle = FontStyle.Italic;

        MapEditorRuntimeControllerInstance.MapSavedEvent += UpdateMapLabel;
        MapEditorRuntimeControllerInstance.MapLoadedEvent += UpdateMapLabel;
        MapEditorRuntimeControllerInstance.MapChangedEvent += UpdateMapLabel;
    }

    void UpdateMapLabel(object sender, Realm realm)
    {
        if (realm != null)
        {
            MapNameLabel.text = GameplayMapBootup.WIPRealm.Name;
            MapNameLabel.fontStyle = FontStyle.Normal;
        }
        else
        {
            MapNameLabel.text = UnnamedDefault;
            MapNameLabel.fontStyle = FontStyle.Italic;
        }
    }
}
