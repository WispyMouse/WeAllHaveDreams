using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public MapHolder MapHolderInstance;

    public float TimeForUnitToWalkAcrossTile = .12f;

    public IEnumerator UnitWalks(MapMob moving, Vector3Int position)
    {
        List<Vector3Int> path = MapHolderInstance.Path(moving, position);

        if (path == null)
        {
            DebugTextLog.AddTextToLog($"{moving.Name} tried to move to a tile that a path couldn't be found for.");
            yield break;
        }

        while (path.Any())
        {
            Vector3Int thisPathPart = path[0];
            path.RemoveAt(0);

            Vector3 targetPosition = new Vector3(thisPathPart.x, thisPathPart.y, thisPathPart.z);
            Vector3 startingPosition = moving.transform.position;
            float curTime = 0;

            while (curTime <= TimeForUnitToWalkAcrossTile)
            {
                moving.transform.position = Vector3.Lerp(startingPosition, targetPosition, curTime / TimeForUnitToWalkAcrossTile);
                curTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            moving.transform.position = targetPosition;
        }
    }
}
