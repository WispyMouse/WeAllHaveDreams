using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamVisibility
{
    public HashSet<Vector3Int> VisibleTiles { get; private set; } = new HashSet<Vector3Int>();
    public HashSet<Vector3Int> HasBeenSeenTiles { get; private set; } = new HashSet<Vector3Int>();

    public void ClearVisibleTiles()
    {
        VisibleTiles = new HashSet<Vector3Int>();
    }

    public void IncorporateVisibleTiles(IEnumerable<Vector3Int> newlyVisible)
    {
        VisibleTiles = new HashSet<Vector3Int>(VisibleTiles.Union(newlyVisible));
        HasBeenSeenTiles = new HashSet<Vector3Int>(HasBeenSeenTiles.Union(newlyVisible));
    }
}
