using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureHolder : MonoBehaviour
{
    public MapHolder MapHolderInstance;
    public Transform FeaturesParent;

    public List<MapFeature> ActiveFeatures { get; set; } = new List<MapFeature>();

    public void SetFeature(Vector3Int position, MapFeature toSet)
    {
        toSet.transform.SetParent(FeaturesParent);
        toSet.SetPosition(position);
        ActiveFeatures.Add(toSet);
    }

    public void ClearAllFeatures()
    {
        foreach (MapFeature curFeature in GameObject.FindObjectsOfType<MapFeature>())
        {
            Destroy(curFeature.gameObject);
        }
        ActiveFeatures = new List<MapFeature>();
    }
}
