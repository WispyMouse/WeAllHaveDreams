using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameplayTile : Tile
{
    public bool CompletelySolid;

    [MenuItem("Assets/Create/GameplayTile")]
    public static void CreateGameplayTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Gameplay Tile", "New Gameplay Tile", "Asset", "Save Gameplay Tile");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameplayTile>(), path);
    }
}
