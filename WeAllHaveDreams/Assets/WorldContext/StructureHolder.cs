using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureHolder : MonoBehaviour
{
    public Transform StructuresParent;

    public List<MapStructure> ActiveStructures { get; set; } = new List<MapStructure>();

    public void SetStructure(StructureMapData data)
    {
        SetStructure(data.Position, StructureLibrary.GetStructure(data.StructureName), FactionHolder.GetPlayer(data.Ownership));
    }

    public void SetStructure(MapCoordinates position, MapStructure toSet, PlayerSide player)
    {
        RemoveStructure(position);

        if (toSet != null)
        {
            toSet.transform.SetParent(StructuresParent);
            toSet.SetPosition(position);
            ActiveStructures.Add(toSet);
        }

        SetOwnership(position, player);
    }

    public void RemoveStructure(MapCoordinates position)
    {
        MapStructure existingStructure = StructureOnPoint(position);

        if (existingStructure != null)
        {
            ActiveStructures.Remove(existingStructure);
            Destroy(existingStructure.gameObject);
        }
    }

    public MapStructure StructureOnPoint(MapCoordinates position)
    {
        MapStructure structureOnPoint;

        if ((structureOnPoint = ActiveStructures.FirstOrDefault(structure => structure.Position == position)) != null)
        {
            return structureOnPoint;
        }

        return null;
    }

    public void ClearAllStructures()
    {
        foreach (MapStructure curStructure in GameObject.FindObjectsOfType<MapStructure>())
        {
            Destroy(curStructure.gameObject);
        }
        ActiveStructures = new List<MapStructure>();
    }

    public void SetOwnership(MapCoordinates position, PlayerSide player)
    {
        MapStructure structureOnPoint = StructureOnPoint(position);

        if (structureOnPoint != null)
        {
            structureOnPoint.SetOwnership(player);
        }
    }

    public void MobRemovedFromPoint(MapCoordinates position)
    {
        MapStructure structure = StructureOnPoint(position);

        if (structure != null)
        {
            structure.ClearCapture();
        }
    }

    public void LoadFromRealm(Realm toLoad)
    {
        foreach (StructureMapData structureData in toLoad.Structures)
        {
            DebugTextLog.AddTextToLog($"Placing {structureData.StructureName} at {structureData.Position.ToString()}, owned by {(structureData.Ownership.HasValue ? $"Faction {structureData.Ownership.Value}" : "[unclaimed]")}", DebugTextLogChannel.Verbose);
            SetStructure(structureData);
        }
    }
}
