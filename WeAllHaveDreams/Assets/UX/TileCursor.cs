using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCursor : MonoBehaviour
{
    public void SetPosition(Vector3Int position)
    {
        transform.position = position;
    }
}
