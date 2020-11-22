using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputGameplayPhase : MonoBehaviour
{
    public virtual void EnterPhase()
    {
    }

    public virtual InputGameplayPhase TileClicked(Vector3Int position)
    {
        return this;
    }

    public virtual InputGameplayPhase UnitClicked(MapMob mob)
    {
        return this;
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
