using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogTile : Tile
{
    public Color FogAlpha;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/FogTile")]
    public static void CreateFogTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Fog Tile", "New Fog Tile", "Asset", "Save Fog Tile");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FogTile>(), path);
    }
#endif

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.color = FogAlpha;
        base.GetTileData(position, tilemap, ref tileData);
    }
}
