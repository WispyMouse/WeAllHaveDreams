using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputGameplayPhase : MonoBehaviour
{
    public virtual IEnumerator EnterPhase()
    {
        yield return new WaitForEndOfFrame();
    }

    public virtual bool TryHandleTileClicked(Vector3Int position, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;
        return false;
    }

    public virtual bool TryHandleUnitClicked(MapMob mob, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;
        return false;
    }

    public virtual bool TryHandleKeyPress(out InputGameplayPhase nextPhase)
    {
        nextPhase = this;
        return false;
    }

    public virtual void EndPhase()
    {

    }

    public virtual void UpdateAfterInput()
    {

    }

    public virtual InputGameplayPhase GetNextPhase()
    {
        return this;
    }

    public virtual bool WaitingForInput => true;

    public virtual bool NextPhasePending => false;
}
