using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public float TimeForUnitToWalkAcrossTile = .12f;

    public IEnumerator UnitWalks(MapMob moving, Vector3Int position)
    {
        List<Vector3Int> path = WorldContextInstance.MapHolder.Path(moving, position);

        if (path == null)
        {
            DebugTextLog.AddTextToLog($"{moving.Name} tried to move to a tile that a path couldn't be found for.");
            yield break;
        }

        Vector3Int startingPosition = moving.Position;

        while (path.Any())
        {
            Vector3Int thisPathPart = path[0];
            path.RemoveAt(0);

            Vector3Int targetPosition = new Vector3Int(thisPathPart.x, thisPathPart.y, thisPathPart.z);
            float curTime = 0;

            WorldContextInstance.FogHolder.ManageVisibilityToCurrentPerspective(moving, targetPosition);

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
