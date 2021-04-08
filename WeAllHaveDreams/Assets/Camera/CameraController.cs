using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : SingletonBase<CameraController>
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();
    Configuration.CameraConfiguration cameraConfiguration;

    public Camera MainCamera;

    public static void CenterCamera()
    {
        Vector3 center = new Vector3(((float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Min(tile => tile.x) + (float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Max(tile => tile.x)) / 2f,
            ((float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Min(tile => tile.y) + (float)Singleton.WorldContextInstance.MapHolder.GetAllTiles().Max(tile => tile.y)) / 2f,
            0);

        Singleton.MainCamera.transform.position = center + Vector3.back * 10;
    }

    public static Vector3 ScreenToWorldPoint(Vector3 pos) => Singleton.MainCamera.ScreenToWorldPoint(pos);

    private void Awake()
    {
        cameraConfiguration = ConfigurationLoadingEntrypoint.GetConfigurationData<Configuration.CameraConfiguration>().First();

        MainCamera.orthographicSize = cameraConfiguration.InitialOrthographicSize;
    }

    private void Update()
    {
        // Don't process camera controls if we have an input active
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            return;
        }

        HandleCameraPanning();
        HandleCameraZoom();
    }

    private void HandleCameraPanning()
    {
        Vector3 inputDirection = Vector3.zero;

        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.y = Input.GetAxis("Vertical");

        // No input, so no panning
        if (inputDirection == Vector3.zero)
        {
            return;
        }

        MainCamera.transform.position = MainCamera.transform.position + inputDirection * Time.deltaTime * cameraConfiguration.CameraPanningSpeed;
    }

    private void HandleCameraZoom()
    {
        // Take the negative of the axis, so that "scrolling down" is interpreted as "increasing orthographic size", which is like zooming out
        float scrollTick = -Input.GetAxis("Mouse ScrollWheel");

        // No input, so no scrolling
        if (scrollTick == 0)
        {
            return;
        }

        float newOrthographicSize = Mathf.Clamp
            (MainCamera.orthographicSize + scrollTick * cameraConfiguration.OrthographicSizeChangePerScrollTick, 
            cameraConfiguration.MinimumOrthographicSize, 
            cameraConfiguration.MaximumOrthographicSize);

        MainCamera.orthographicSize = newOrthographicSize;
    }
}
