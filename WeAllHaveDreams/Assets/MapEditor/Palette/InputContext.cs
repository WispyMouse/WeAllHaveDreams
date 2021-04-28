using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputContext
{
    public MapCoordinates StartClick { get; set; }
    public MapCoordinates? EndClick { get; set; }

    public InputContext(MapCoordinates start)
    {
        StartClick = start;
    }
}
