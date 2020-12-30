using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Realm
{
    // These control how many pixels per coordinate there are in the MapImage.
    const int PixelWidthPerTile = 2;
    const int PixelHeightPerTile = 2;

    public string MapImage;
    public IEnumerable<RealmKey> Keys;
    Dictionary<Color, RealmKey> ColorsToKeys { get; set; }

    public HashSet<Vector3Int> AllPositions { get; private set; }
    public Dictionary<Vector3Int, IEnumerable<Vector3Int>> Neighbors { get; private set; }
    public Dictionary<Vector3Int, HashSet<RealmKey>> KeysAtPositions { get; private set; }

    public async Task Hydrate()
    {
        await GenerateMapFromImage();
        GenerateNeighbors();
    }

    async Task GenerateMapFromImage()
    {
        KeysAtPositions = new Dictionary<Vector3Int, HashSet<RealmKey>>();
        ColorsToKeys = new Dictionary<Color, RealmKey>();

        foreach (RealmKey key in Keys)
        {
            if (ColorUtility.TryParseHtmlString(key.Color, out Color parsedColor))
            {
                ColorsToKeys.Add(parsedColor, key);
            }
            else
            {
                DebugTextLog.AddTextToLog($"Could not parse color '{key.Color}'");
            }
        }

        string mapImagePath = Path.Combine(MapBootup.MapFolderPath, MapImage);

        if (!File.Exists(mapImagePath))
        {
            string errorMessage = $"No map was found at {mapImagePath}.";
            DebugTextLog.AddTextToLog(errorMessage);
            throw new System.Exception(errorMessage);
        }

        ResourceRequest request = Resources.LoadAsync<Sprite>("Maps/DemoMap");

        while (!request.isDone)
        {
            await Task.Delay(1);
        }

        Sprite loadedSprite = request.asset as Sprite;
        
        for (int xx = 0; xx < loadedSprite.rect.width / PixelWidthPerTile; xx++)
        {
            for (int yy = 0; yy < loadedSprite.rect.height / PixelHeightPerTile; yy++)
            {
                Vector3Int mapCoordinate = new Vector3Int(xx, yy, 0);
                HashSet<Color> colors = new HashSet<Color>();

                // Every 2x2 block is one tile. (or whatever is configured in PixelWidthPerTile and PixelHeightPerTile)
                // Everything indicated in the key should be put in to this coordinate
                foreach (Vector2Int pixelCoordinate in PointsForCoordinate(mapCoordinate))
                {
                    colors.Add(loadedSprite.texture.GetPixel(pixelCoordinate.x, pixelCoordinate.y));
                }

                KeysAtPositions.Add(mapCoordinate, new HashSet<RealmKey>());

                foreach (Color color in colors)
                {
                    KeysAtPositions[mapCoordinate].Add(ColorsToKeys[color]);
                }
            }
        }
    }

    IEnumerable<Vector2Int> PointsForCoordinate(Vector3Int mapCoordinate)
    {
        // Given a starting left and top, what are the pixel coordinates in the GameMap that should correspond to it?
        return Enumerable.Range(mapCoordinate.x * PixelWidthPerTile, PixelWidthPerTile)
            .SelectMany(x => Enumerable.Range(mapCoordinate.y * PixelHeightPerTile, PixelHeightPerTile)
            .Select(y => new Vector2Int(x, y)));
    }

    void GenerateNeighbors()
    {
        AllPositions = new HashSet<Vector3Int>(KeysAtPositions.Keys);
        Neighbors = new Dictionary<Vector3Int, IEnumerable<Vector3Int>>();

        foreach (Vector3Int position in AllPositions)
        {
            List<Vector3Int> neighbors = new List<Vector3Int>();

            foreach (Vector3Int potentialNeighbor in GameMap.GetPotentialNeighbors(position))
            {
                if (AllPositions.Contains(potentialNeighbor))
                {
                    neighbors.Add(potentialNeighbor);
                }
            }

            Neighbors.Add(position, neighbors);
        }
    }
}
