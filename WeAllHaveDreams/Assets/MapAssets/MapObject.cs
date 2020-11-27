using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public Vector3Int Position { get; set; }

    public void SettleIntoGrid()
    {
        Vector3Int nearestStartPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        SetPosition(nearestStartPosition);
    }

    public void SetPosition(Vector3Int toPosition)
    {
        Position = toPosition;
        transform.position = toPosition;
    }
}
