using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public float TimeForUnitToWalkAcrossTile = .12f;

    public IEnumerator UnitWalks(MapMob moving, MapCoordinates position)
    {
        List<MapCoordinates> path = WorldContextInstance.MapHolder.Path(moving, position);

        if (path == null)
        {
            DebugTextLog.AddTextToLog($"{moving.Name} tried to move to a tile that a path couldn't be found for.");
            yield break;
        }

        Vector3 startingPosition = new Vector3(moving.Position.X, moving.Position.Y);

        while (path.Any())
        {
            MapCoordinates thisPathPart = path[0];
            path.RemoveAt(0);

            Vector3 targetPosition = new Vector3(thisPathPart.X, thisPathPart.Y, 0);
            float curTime = 0;

            WorldContextInstance.FogHolder.ManageVisibilityToCurrentPerspective(moving, thisPathPart);

            while (curTime <= TimeForUnitToWalkAcrossTile)
            {
                moving.transform.position = Vector3.Lerp(startingPosition, targetPosition, curTime / TimeForUnitToWalkAcrossTile);
                curTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            moving.transform.position = targetPosition;
            startingPosition = targetPosition;
        }
    }
}
