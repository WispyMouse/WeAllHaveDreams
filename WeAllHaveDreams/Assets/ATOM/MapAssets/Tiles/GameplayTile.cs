using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameplayTile : Tile
{
    public TileConfiguration Configuration;
    public string TileName => Configuration.TileName;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        HashSet<NeighborDirection> occuppiedNeighbors = GetNeighborsDirectionsWithSameTile(position, tilemap);

        TileNeighborSpriteSetting matchingSetting = GetBestNeighborSpriteSetting(occuppiedNeighbors);
        string spriteToUse = matchingSetting?.SpriteToUse;

        if (string.IsNullOrEmpty(spriteToUse))
        {
            spriteToUse = Configuration.DefaultSprite;
        }

        tileData.sprite = TileLibrary.GetSprite(spriteToUse);
    }

    public void LoadFromConfiguration(TileConfiguration configuration)
    {
        Configuration = configuration;
    }

    public MovementCostAttribute MovementCosts(MapMob forMob)
    {
        // If we have no attributes, then return null to indicate there is no modification
        if (Configuration.MovementCostAttributes == null || !Configuration.MovementCostAttributes.Any())
        {
            return null;
        }
        MovementCostAttribute chosenAttribute = null;

        foreach (MovementCostAttribute movement in Configuration.MovementCostAttributes.OrderByDescending(move => move.Priority))
        {
            if (movement.TagsApply(forMob.Tags))
            {
                return movement;
            }
        }

        return chosenAttribute;
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
        return Configuration.SpriteSettings
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
