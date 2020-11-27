using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIInputPhaseController : MonoBehaviour
{
    Coroutine TurnBeingPlayed { get; set; }

    public MobHolder MobHolderInstance;
    public MapHolder MapHolderInstance;
    public MapMeta MapMetaInstance;

    public float TimeToWaitAfterSelectingUnit = .4f;
    public float TimeToWaitAfterMovingUnit = .15f;

    public void StartTurn()
    {
        TurnBeingPlayed = StartCoroutine(ConductTurn());
    }

    IEnumerator ConductTurn()
    {
        MapMob matchingMob;

        while ((matchingMob = MobHolderInstance.MobsOnTeam(TurnManager.CurrentPlayer.PlayerSideIndex).Where(mob => mob.CanMove || mob.CanAttack).FirstOrDefault()) != null
            && TurnManager.GameIsInProgress)
        {
            yield return ActWithUnit(matchingMob);
            // TEMPORARY: Force set the Mob's Cans to false, so we can move on in the AI
            matchingMob.ExhaustAllOptions();
        }

        TurnManager.PassTurnToNextPlayer();
    }

    IEnumerator ActWithUnit(MapMob acting)
    {
        MapMetaInstance.ShowUnitAttackRangePastMovementRange(acting);

        yield return new WaitForSeconds(TimeToWaitAfterSelectingUnit);
        MapMetaInstance.ClearMetas();

        // Simple AI: Find the nearest unit not on our team
        // Move as close as we can to them
        // If we can attack, attack
        IEnumerable<MapMob> nearestEnemies = MobHolderInstance.ActiveMobs.Where(mob => mob.PlayerSideIndex != TurnManager.CurrentPlayer.PlayerSideIndex)
            .OrderBy(mob => Mathf.Abs(mob.Position.x - acting.Position.x) + Mathf.Abs(mob.Position.y - acting.Position.y));

        IEnumerable<Vector3Int> movementRange = MapHolderInstance.PotentialMoves(acting);

        if (!acting.CanMove)
        {
            movementRange = new List<Vector3Int>() { acting.Position };
        }

        foreach (MapMob nearestEnemy in nearestEnemies)
        {
            // Assume every tile we can hit someone from, we could also hit from that tile
            IEnumerable<Vector3Int> canBeHurtFrom = MapHolderInstance.CanHitFrom(acting, nearestEnemy.Position);
            IEnumerable<Vector3Int> intersect;
            if ((intersect = movementRange.Intersect(canBeHurtFrom)).Any())
            {
                if (acting.CanMove)
                {
                    Vector3Int closestToCurrent = intersect.OrderBy(pos => Mathf.Abs(acting.Position.x - pos.x) + Mathf.Abs(acting.Position.y - pos.y)).First();

                    if (closestToCurrent != acting.Position)
                    {
                        yield return new MoveMobPlayerInput(acting, closestToCurrent).Execute(MapHolderInstance, MobHolderInstance);
                        yield return new WaitForSeconds(TimeToWaitAfterMovingUnit);
                    }
                }
                
                if (acting.CanAttack && intersect.Contains(acting.Position))
                {
                    yield return new AttackWithMobInput(acting, nearestEnemy).Execute(MapHolderInstance, MobHolderInstance);
                    break;
                }
            }
            else if (canBeHurtFrom.Any() && acting.CanMove)
            {
                Vector3Int closestCanBeHurtFrom = canBeHurtFrom.OrderBy(hurtFrom => Mathf.Abs(acting.Position.x - hurtFrom.x) + Mathf.Abs(acting.Position.y - hurtFrom.y)).First();
                List<Vector3Int> pathTowardsClosest = MapHolderInstance.Path(acting, closestCanBeHurtFrom);

                if (pathTowardsClosest != null)
                {
                    IEnumerable<Vector3Int> intersectOfPath = pathTowardsClosest.Intersect(movementRange);

                    for (int ii = pathTowardsClosest.Count - 1; ii >= 0; ii--)
                    {
                        if (intersectOfPath.Contains(pathTowardsClosest[ii]))
                        {
                            yield return new MoveMobPlayerInput(acting, pathTowardsClosest[ii]).Execute(MapHolderInstance, MobHolderInstance);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void StopAllInputs()
    {
        StopCoroutine(TurnBeingPlayed);
    }
}
