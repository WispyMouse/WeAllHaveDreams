using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from https://wiki.unity3d.com/index.php/Bresenham3D
public static class BresenhamLineDrawer
{
    public static List<MapCoordinates> PointsOnLine(MapCoordinates start, MapCoordinates end)
    {
        List<MapCoordinates> points = new List<MapCoordinates>();

        int xd, yd;
        int x, y;
        int ax, ay;
        int sx, sy;
        int dx, dy;

        dx = (int)(end.X - start.X);
        dy = (int)(end.Y - start.Y);

        ax = Mathf.Abs(dx) << 1;
        ay = Mathf.Abs(dy) << 1;

        sx = (int)Mathf.Sign((float)dx);
        sy = (int)Mathf.Sign((float)dy);

        x = (int)start.X;
        y = (int)start.Y;

        if (ax >= ay) // x dominant
        {
            yd = ay - (ax >> 1);
            for (; ; )
            {
                points.Add(new MapCoordinates(x, y));

                if (x == (int)end.X)
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
                points.Add(new MapCoordinates(x, y));

                if (y == (int)end.Y)
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
