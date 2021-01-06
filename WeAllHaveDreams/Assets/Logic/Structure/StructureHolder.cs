using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureHolder : MonoBehaviour
{
    public MapHolder MapHolderInstance;
    public Transform StructuresParent;

    public List<MapStructure> ActiveStructures { get; set; } = new List<MapStructure>();

    public void SetStructure(Vector3Int position, MapStructure toSet)
    {
        toSet.transform.SetParent(StructuresParent);
        toSet.SetPosition(position);
        ActiveStructures.Add(toSet);
    }

    public void LoadStructuresFromScene()
    {
        ActiveStructures = new List<MapStructure>();

        foreach (MapStructure curStructure in GameObject.FindObjectsOfType<MapStructure>())
        {
            curStructure.SettleIntoGrid();

            if (ActiveStructures.Any(structure => structure.Position == curStructure.Position))
            {
                Debug.LogWarning($"Multiple structures are on the same position: {{{curStructure.Position.x}, {curStructure.Position.y}, {curStructure.Position.z}}}");
            }

            ActiveStructures.Add(curStructure);
        }
    }

    public MapStructure StructureOnPoint(Vector3Int position)
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

    public void SetOwnership(Vector3Int position, int team)
    {
        MapStructure structureOnPoint = ActiveStructures.FirstOrDefault(structure => structure.Position == position);

        if (structureOnPoint != null)
        {
            structureOnPoint.SetOwnership(team);
        }
    }
}
