using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMob : MonoBehaviour
{
    public Vector3Int Position { get; set; }
    public int MoveRange => 3; // TEMPORARY: Just a static move value

    private void Start()
    {
        Vector3Int nearestStartPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        SetPosition(nearestStartPosition);
    }

    public void SetPosition(Vector3Int toPosition)
    {
        Position = toPosition;
    }
}
