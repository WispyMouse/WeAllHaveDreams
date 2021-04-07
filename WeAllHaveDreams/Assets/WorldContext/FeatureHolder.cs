using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatureHolder : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();
    public Transform FeaturesParent;

    public List<MapFeature> ActiveFeatures { get; set; } = new List<MapFeature>();

    public void SetFeature(FeatureMapData mapData)
    {
        SetFeature(mapData.Position, FeatureLibrary.GetFeature(mapData.FeatureName));
    }

    public void SetFeature(Vector3Int position, MapFeature toSet)
    {
        MapFeature existingFeature = FeatureOnPoint(position);

        if (existingFeature != null)
        {
            ActiveFeatures.Remove(existingFeature);
            Destroy(existingFeature.gameObject);
        }

        if (toSet != null)
        {
            toSet.transform.SetParent(FeaturesParent);
            toSet.SetPosition(position);
            ActiveFeatures.Add(toSet);

            MapMob onTile = WorldContextInstance.MobHolder.MobOnPoint(position);
            if (onTile != null)
            {
                onTile.CalculateStandingStatAdjustments(toSet);
            }
        }

        if (TurnManager.GameIsInProgress)
        {
            WorldContextInstance.FogHolder.UpdateVisibilityForPlayer(TurnManager.CurrentPlayer.PlayerSideIndex);
        }
    }

    public MapFeature FeatureOnPoint(Vector3Int position)
    {
        MapFeature featureOnPoint;

        if ((featureOnPoint = ActiveFeatures.FirstOrDefault(feature => feature.Position == position)) != null)
        {
            return featureOnPoint;
        }

        return null;
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
