using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public float TimeToImpact = .16f;
    public float TimeForPauseAfterImpact = .2f;
    public float TimeForRetract = .2f;

    public float ClosenessPercentile = .65f;

    public IEnumerator UnitAttacksUnit(MapMob aggresser, MapMob defender, System.Action impactAction)
    {
        Vector3 startingPosition = aggresser.transform.position;
        Vector3 targetPosition = Vector3.Lerp(aggresser.transform.position, defender.transform.position, ClosenessPercentile);

        float curGoTime = 0;
        while (curGoTime < TimeToImpact)
        {
            yield return new WaitForEndOfFrame();
            curGoTime += Time.deltaTime;
            aggresser.transform.position = Vector3.Lerp(startingPosition, targetPosition, curGoTime / TimeToImpact);
        }

        aggresser.transform.position = targetPosition;

        impactAction.Invoke();

        yield return new WaitForSeconds(TimeForPauseAfterImpact);

        float curReturnTime = 0;
        while (curReturnTime < TimeForRetract)
        {
            yield return new WaitForEndOfFrame();
            curReturnTime += Time.deltaTime;
            aggresser.transform.position = Vector3.Lerp(targetPosition, startingPosition, curReturnTime / TimeForRetract);
        }

        aggresser.transform.position = startingPosition;
    }
}
