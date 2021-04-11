using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public MapCoordinates Position { get; set; }
    public virtual IEnumerable<string> Tags { get; }

    bool shouldBeVisible { get; set; }

    public void SettleIntoGrid()
    {
        MapCoordinates nearestStartPosition = new MapCoordinates(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        SetPosition(nearestStartPosition);
    }

    public virtual void SetPosition(MapCoordinates toPosition)
    {
        Position = toPosition;
        transform.position = new Vector3(toPosition.X, toPosition.Y, 0);
    }

    public virtual bool HasStartOfTurnEffects => false;

    public virtual void StartOfTurnEffects(MapMob mobOnTile)
    {

    }

    public virtual IEnumerable<StatAdjustment> StatAdjustmentsForMob(MapMob mobOnTile)
    {
        return new List<StatAdjustment>();
    }

    public virtual void HideDueToFog()
    {
        gameObject.SetActive(false);
        shouldBeVisible = false;
    }

    public virtual void BeVisible()
    {
        gameObject.SetActive(true);
        shouldBeVisible = true;
    }
}
