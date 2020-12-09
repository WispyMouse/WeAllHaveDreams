using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from https://wiki.unity3d.com/index.php/Bresenham3D
public static class BresenhamLineDrawer
{
    public static List<Vector3Int> PointsOnLine(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> points = new List<Vector3Int>();

        int xd, yd;
        int x, y;
        int ax, ay;
        int sx, sy;
        int dx, dy;

        dx = (int)(end.x - start.x);
        dy = (int)(end.y - start.y);

        ax = Mathf.Abs(dx) << 1;
        ay = Mathf.Abs(dy) << 1;

        sx = (int)Mathf.Sign((float)dx);
        sy = (int)Mathf.Sign((float)dy);

        x = (int)start.x;
        y = (int)start.y;

        if (ax >= ay) // x dominant
        {
            yd = ay - (ax >> 1);
            for (; ; )
            {
                points.Add(new Vector3Int(x, y, 0));

                if (x == (int)end.x)
                {
                    return points;
                }

                if (yd >= 0)
                {
                    y += sy;
                    yd -= ax;
                }

                x += sx;
                yd += ay;
            }
        }
        else if (ay >= ax) // y dominant
        {
            xd = ax - (ay >> 1);
            for (; ; )
            {
                points.Add(new Vector3Int(x, y, 0));

                if (y == (int)end.y)
                    return points;

                if (xd >= 0)
                {
                    x += sx;
                    xd -= ay;
                }

                y += sy;
                xd += ax;
            }
        }

        return points;
    }
}
