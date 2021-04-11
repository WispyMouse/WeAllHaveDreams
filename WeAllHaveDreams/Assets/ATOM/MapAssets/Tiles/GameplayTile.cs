using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameplayTile : Tile
{
    public bool CompletelySolid;
    public string TileName;

    public TileNeighborSpriteSetting[] SpriteSettings;
    public Sprite DefaultSprite;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/GameplayTile")]
    public static void CreateGameplayTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Gameplay Tile", "New Gameplay Tile", "Asset", "Save Gameplay Tile");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameplayTile>(), path);
    }
#endif

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        HashSet<NeighborDirection> occuppiedNeighbors = GetNeighborsDirectionsWithSameTile(position, tilemap);
        TileNeighborSpriteSetting matchingSetting = GetBestNeighborSpriteSetting(occuppiedNeighbors);

        if (matchingSetting != null)
        {
            tileData.sprite = matchingSetting.SpriteToUse ;
        }
        else
        {
            tileData.sprite = DefaultSprite;
        }
    }

    HashSet<NeighborDirection> GetNeighborsDirectionsWithSameTile(MapCoordinates position, ITilemap tilemap)
    {
        HashSet<NeighborDirection> occuppiedNeighbors = new HashSet<NeighborDirection>();

        foreach (NeighborDirection direction in System.Enum.GetValues(typeof(NeighborDirection)))
        {
            MapCoordinates neighborPosition = PositionInDirection(position, direction);
            GameplayTile neighborTile;

            if (neighborTile = tilemap.GetTile<GameplayTile>(neighborPosition))
            {
                if (neighborTile.TileName == this.TileName)
                {
                    occuppiedNeighbors.Add(direction);
                }
            }
        }

        return occuppiedNeighbors;
    }

    TileNeighborSpriteSetting GetBestNeighborSpriteSetting(HashSet<NeighborDirection> occuppiedNeighbors)
    {
        return SpriteSettings
            .Where(setting => setting.SameNeighborDirections.Length == occuppiedNeighbors.Count)
            .Where(setting => setting.SameNeighborDirections.All(snd => occuppiedNeighbors.Contains(snd)))
            .FirstOrDefault();
    }

    static MapCoordinates PositionInDirection(MapCoordinates start, NeighborDirection direction)
    {
        switch (direction)
        {
            case NeighborDirection.North:
                return start + MapCoordinates.Up;
            case NeighborDirection.East:
                return start + MapCoordinates.Right;
            case NeighborDirection.South:
                return start + MapCoordinates.Down;
            case NeighborDirection.West:
                return start + MapCoordinates.Left;
            default:
                return start;
        }
    }
}
