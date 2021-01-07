using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIInputPhaseController : MonoBehaviour
{
    Coroutine TurnBeingPlayed { get; set; }

    public MobHolder MobHolderInstance;
    public MapHolder MapHolderInstance;
    public StructureHolder StructureHolderInstance;
    public MapMeta MapMetaInstance;

    public float TimeToWaitAfterSelectingUnit = .4f;
    public float TimeToWaitAfterMovingUnit = .15f;

    List<MapMob> mobsUndecided = new List<MapMob>();

    public void StartTurn()
    {
        TurnBeingPlayed = StartCoroutine(ConductTurn());
    }

    IEnumerator ConductTurn()
    {
        IEnumerable<MapMob> remainingActors;
        while ((remainingActors = MobHolderInstance.MobsOnTeam(TurnManager.CurrentPlayer.PlayerSideIndex).Where(mob => mob.CanMove || mob.CanAttack)).Any())
        {
            List<UnitTurnPlan> possiblePlans = new List<UnitTurnPlan>();

            foreach (MapMob curMob in remainingActors)
            {
                possiblePlans.Add(GetBestPersonalPlan(curMob));
            }

            UnitTurnPlan bestPlan = possiblePlans.OrderByDescending(plan => plan.Score).First();
            DebugTextLog.AddTextToLog("determined best plan, acting");
            yield return bestPlan.DeterminedInput.Execute(MapHolderInstance, MobHolderInstance);
        }

        foreach (MapStructure curStructure in StructureHolderInstance.ActiveStructures.Where(structure => !structure.UnCaptured && structure.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex))
        {
            if (!MobHolderInstance.MobOnPoint(curStructure.Position))
            {
                IEnumerable<PlayerInput> possibleInputs = curStructure.GetPossiblePlayerInputs(MobHolderInstance)
                    .Where(input => input.IsPossible());

                // Zombie behavior: Randomly pick from possible inputs
                if (possibleInputs.Any())
                {
                    PlayerInput randomInput = possibleInputs.ToList()[Random.Range(0, possibleInputs.Count())];
                    yield return randomInput.Execute(MapHolderInstance, MobHolderInstance);
                }
            }
        }

        TurnManager.PassTurnToNextPlayer();
    }

    UnitTurnPlan GetBestPersonalPlan(MapMob acting)
    {
        List<UnitTurnPlan> possiblePlans = new List<UnitTurnPlan>();
        possiblePlans.Add(new UnitTurnPlan(new DoesNothingPlayerInput(acting), 0));

        // Get our current movement and possible hurt ranges
        IEnumerable<Vector3Int> movementRanges = MapHolderInstance.PotentialMoves(acting);
        IEnumerable<Vector3Int> hurtRanges = movementRanges.SelectMany(mr => MapHolderInstance.PotentialAttacks(acting, mr));

        if (!acting.CanMove)
        {
            movementRanges = new List<Vector3Int>() { acting.Position };
        }

        if (acting.CanAttack)
        {
            // What opponents are in our hurt range?
            IEnumerable<MapMob> opponentsInRange = MobHolderInstance.ActiveMobs
                .Where(mob => mob.PlayerSideIndex != acting.PlayerSideIndex)
                .Where(mob => hurtRanges.Contains(mob.Position));

            if (opponentsInRange.Any())
            {
                foreach (MapMob inRange in opponentsInRange)
                {
                    IEnumerable<Vector3Int> engagementTiles = MapHolderInstance.CanHitFrom(acting, inRange.Position)
                        .Intersect(movementRanges);

                    if (!engagementTiles.Any())
                    {
                        continue;
                    }

                    IEnumerable<Vector3Int> emptyEngagementTiles = engagementTiles.Where(tile => acting.Position == tile || MobHolderInstance.MobOnPoint(tile) == null);

                    if (!emptyEngagementTiles.Any())
                    {
                        continue;
                    }

                    possiblePlans.Add(new UnitTurnPlan(new AttackWithMobInput(acting, inRange, emptyEngagementTiles.First()), ScoreEngagement(acting, inRange)));
                }
            }
        }        

        // Are there any structures in our movement range?
        IEnumerable<MapStructure> structuresInRange = StructureHolderInstance.ActiveStructures
            .Where(structure => movementRanges.Contains(structure.Position))
            .Where(structure => structure.PlayerSideIndex != acting.PlayerSideIndex || structure.UnCaptured)
            .Where(structure => acting.Position == structure.Position || MobHolderInstance.MobOnPoint(structure.Position) == null);

        if (structuresInRange.Any())
        {
            foreach (MapStructure structure in structuresInRange)
            {
                int sameTileModifier = structure.Position == acting.Position ? 1 : 0;
                possiblePlans.Add(new UnitTurnPlan(new MobCapturesStructurePlayerInput(acting, structure), ScoreCapturing(acting, structure) + sameTileModifier));
            }
        }

        if (acting.CanMove)
        {
            // If the enemy has a base, move towards it
            IEnumerable<MapStructure> enemyStructures = StructureHolderInstance.ActiveStructures
                .Where(structure => structure.UnCaptured || structure.PlayerSideIndex != TurnManager.CurrentPlayer.PlayerSideIndex)
                .Except(structuresInRange);

            foreach (MapStructure structure in enemyStructures)
            {
                List<Vector3Int> path = MapHolderInstance.Path(acting, structure.Position);

                if (path == null)
                {
                    continue;
                }

                for (int ii = Mathf.Min(path.Count - 1, acting.MoveRange); ii >= 0; ii--)
                {
                    List<Vector3Int> pathToSpot = MapHolderInstance.Path(acting, path[ii]);

                    if (pathToSpot == null)
                    {
                        continue;
                    }

                    int distanceModifier = ii;
                    possiblePlans.Add(new UnitTurnPlan(new MoveMobPlayerInput(acting, path[ii]), Mathf.FloorToInt((ScoreCapturing(acting, structure) - distanceModifier) * .25f)));

                    break;
                }
            }
        }

        return possiblePlans.OrderByDescending(plan => plan.Score).First();
    }

    int ScoreEngagement(MapMob acting, MapMob defending)
    {
        decimal projectedOutgoingDamage = MobHolderInstance.ProjectedDamages(acting, defending);

        if (projectedOutgoingDamage >= defending.HitPoints)
        {
            return (int)defending.HitPoints + 10;
        }

        decimal returnDamage = MobHolderInstance.ProjectedDamages(defending, acting, defending.HitPoints - projectedOutgoingDamage);

        return (int)(projectedOutgoingDamage - returnDamage);
    }

    int ScoreCapturing(MapMob acting, MapStructure structure)
    {
        decimal projectedCapturePower = acting.CurrentCapturePoints;

        if (projectedCapturePower > structure.CurCapturePoints)
        {
            return structure.CurCapturePoints + structure.CaptureImportance;
        }

        return (int)projectedCapturePower + structure.CaptureImportance;
    }

    public void StopAllInputs()
    {
        StopCoroutine(TurnBeingPlayed);
    }
}
