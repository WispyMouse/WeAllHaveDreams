using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCursor : MonoBehaviour
{
    public void SetPosition(MapCoordinates position)
    {
        transform.position = new Vector3(position.X, position.Y, 0);
    }
}
