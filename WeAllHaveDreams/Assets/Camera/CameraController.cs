using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : SingletonBase<CameraController>
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public Camera MainCamera;

    public static void CenterCamera()
    {
        Vector3 center = new Vector3(((float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Min(tile => tile.x) + (float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Max(tile => tile.x)) / 2f,
            ((float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Min(tile => tile.y) + (float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Max(tile => tile.y)) / 2f,
            0);

        Singleton.MainCamera.transform.position = center + Vector3.back * 10;
    }

    public static Vector3 ScreenToWorldPoint(Vector3 pos) => Singleton.MainCamera.ScreenToWorldPoint(pos);
}
