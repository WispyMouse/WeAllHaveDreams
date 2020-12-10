using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Realm
{
    public string MapString;

    public HashSet<Vector3Int> AllPositions { get; private set; }
    public Dictionary<Vector3Int, IEnumerable<Vector3Int>> Neighbors { get; private set; }

    Dictionary<Vector3Int, char> Chars { get; set; }
    Dictionary<char, string> Tags { get; set; }

    public void Hydrate()
    {
        GenerateTags();
        GenerateCharsMap();
        GenerateNeighbors();
    }

    void GenerateTags()
    {
        Tags = new Dictionary<char, string>();

        Tags.Add('_', "Floor");
    }

    void GenerateCharsMap()
    {
        AllPositions = new HashSet<Vector3Int>();
        Chars = new Dictionary<Vector3Int, char>();

        string[] splitProtoMap = MapString.Split('\n');
        int longestWidth = splitProtoMap.Max(line => line.Length);

        for (int yy = 0; yy < splitProtoMap.Length; yy++)
        {
            string thisLine = splitProtoMap[yy];

            for (int xx = 0; xx < longestWidth; xx++)
            {
                Vector3Int thisPosition = new Vector3Int(xx, yy, 0);

                AllPositions.Add(thisPosition);

                if (thisLine.Length <= xx)
                {
                    Chars.Add(thisPosition, '_');
                }
                else
                {
                    Chars.Add(thisPosition, thisLine[xx]);
                }
            }
        }
    }
    void GenerateNeighbors()
    {
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

    public string TileTagAtPosition(Vector3Int position)
    {
        return Tags[Chars[position]];
    }
}
