using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamVisibility
{
    public HashSet<MapCoordinates> VisibleCoordinates { get; private set; } = new HashSet<MapCoordinates>();
    public HashSet<MapCoordinates> HasBeenSeenCoordinates { get; private set; } = new HashSet<MapCoordinates>();

    public void ClearVisibleTiles()
    {
        VisibleCoordinates = new HashSet<MapCoordinates>();
    }

    public void IncorporateVisibleTiles(IEnumerable<MapCoordinates> newlyVisible)
    {
        VisibleCoordinates = new HashSet<MapCoordinates>(VisibleCoordinates.Union(newlyVisible));
        HasBeenSeenCoordinates = new HashSet<MapCoordinates>(HasBeenSeenCoordinates.Union(newlyVisible));
    }
}
