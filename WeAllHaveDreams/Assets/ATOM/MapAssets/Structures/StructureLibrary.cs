using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureLibrary : SingletonBase<StructureLibrary>
{
    public MapStructure[] Structures;
    Dictionary<string, MapStructure> NamesToStructures { get; set; } = new Dictionary<string, MapStructure>();

    public MapStructure DefaultStructure;

    void Awake()
    {
        ExplicitlySetSingleton();
    }

    public static MapStructure GetStructure(string structureName)
    {
        if (string.IsNullOrEmpty(structureName))
        {
            return null;
        }

        if (Singleton.NamesToStructures.TryGetValue(structureName, out MapStructure foundStructure))
        {
            return Instantiate(foundStructure);
        }

        MapStructure matchingStructure = Singleton.Structures.FirstOrDefault(structure => structure.StructureName == structureName);

        if (matchingStructure == null)
        {
            DebugTextLog.AddTextToLog($"Could not find a Structure in the Library with the name {structureName}. Returning a default.");
            return Instantiate(Singleton.DefaultStructure);
        }

        Singleton.NamesToStructures.Add(structureName, matchingStructure);
        return Instantiate(matchingStructure);
    }

    public static IEnumerable<MapStructure> GetAllStructures()
    {
        return Singleton.Structures;
    }
}
