using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A position in space.
/// This is used instead of <see cref="Vector3Int"/> because some properties are serialized, making the data a little less clean.
/// </summary>
[System.Serializable]
public struct MapCoordinates : IEquatable<MapCoordinates>
{
    /// <summary>
    /// The X coordinate of the position. Relates to horizontal space.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The Y 
    /// </summary>
    public int Y { get; set; }

    public MapCoordinates(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static MapCoordinates Up
    {
        get
        {
            return new MapCoordinates(0, 1);
        }
    }

    public static MapCoordinates Right
    {
        get
        {
            return new MapCoordinates(1, 0);
        }
    }

    public static MapCoordinates Down
    {
        get
        {
            return new MapCoordinates(0, -1);
        }
    }

    public static MapCoordinates Left
    {
        get
        {
            return new MapCoordinates(-1, 0);
        }
    }

    public static MapCoordinates Zero
    {
        get
        {
            return new MapCoordinates(0, 0);
        }
    }

    public static MapCoordinates operator +(MapCoordinates a, MapCoordinates b)
    {
        return new MapCoordinates(a.X + b.X, a.Y + b.Y);
    }

    public static bool operator ==(MapCoordinates a, MapCoordinates b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(MapCoordinates a, MapCoordinates b)
    {
        return !(a == b);
    }

    public static implicit operator Vector3Int(MapCoordinates coordinate)
    {
        return new Vector3Int(coordinate.X, coordinate.Y, 0);
    }

    public static implicit operator MapCoordinates(Vector3Int position)
    {
        return new MapCoordinates(position.x, position.y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public override bool Equals(object obj)
    {
        if (obj is MapCoordinates)
        {
            return this == (MapCoordinates)obj;
        }
        else if (obj is Vector3Int)
        {
            return this == (MapCoordinates)obj;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return X ^ Y;
    }

    public bool Equals(MapCoordinates other)
    {
        return this == other;
    }
}
