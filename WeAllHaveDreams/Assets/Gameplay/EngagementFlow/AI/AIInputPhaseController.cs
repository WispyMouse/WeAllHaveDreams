using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIInputPhaseController : MonoBehaviour
{
    Coroutine TurnBeingPlayed { get; set; }

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();
    public GameplayAnimationHolder GameplayAnimationInstance;

    public Configuration.AIConfiguration AISettings;

    List<MapMob> mobsUndecided = new List<MapMob>();

    public void LoadSettings()
    {
        AISettings = ConfigurationLoadingEntrypoint.GetConfigurationData<Configuration.AIConfiguration>().First();
    }

    public void StartTurn()
    {
        TurnBeingPlayed = StartCoroutine(ConductTurn());
    }

    IEnumerator ConductTurn()
    {
        IEnumerable<MapMob> remainingActors;
        while ((remainingActors = WorldContextInstance.MobHolder.MobsOnTeam(TurnManager.CurrentPlayer.PlayerSideIndex).Where(mob => mob.CanMove || mob.CanAttack)).Any())
        {
            List<UnitTurnPlan> possiblePlans = new List<UnitTurnPlan>();

            foreach (MapMob curMob in remainingActors)
            {
                possiblePlans.Add(GetBestPersonalPlan(curMob));
            }

            UnitTurnPlan bestPlan = possiblePlans.OrderByDescending(plan => plan.Score).First();
            DebugTextLog.AddTextToLog($"AI Plan: {bestPlan.DeterminedInput.LongTitle}, score {bestPlan.Score}");

            if (!bestPlan.DeterminedInput.IsPossible(WorldContextInstance))
            {
                DebugTextLog.AddTextToLog($"AI Plan is not possible to perform. Exhausting next available unit and moving on.", DebugTextLogChannel.AILogging);
                yield return new DoesNothingPlayerInput(remainingActors.First()).Execute(WorldContextInstance, GameplayAnimationInstance);
            }
            else
            {
                yield return bestPlan.DeterminedInput.Execute(WorldContextInstance, GameplayAnimationInstance);
            }
        }

        foreach (MapStructure curStructure in WorldContextInstance.StructureHolder.ActiveStructures.Where(structure => structure.PlayerSideIndex == TurnManager.CurrentPlayer.PlayerSideIndex))
        {
            if (!WorldContextInstance.MobHolder.MobOnPoint(curStructure.Position))
            {
                IEnumerable<PlayerInput> possibleInputs = curStructure.GetPossiblePlayerInputs(WorldContextInstance)
                    .Where(input => input.IsPossible(WorldContextInstance));

                // Zombie behavior: Randomly pick from possible inputs
                if (possibleInputs.Any())
                {
                    PlayerInput randomInput = possibleInputs.ToList()[Random.Range(0, possibleInputs.Count())];

                    if (!randomInput.IsPossible(WorldContextInstance))
                    {
                        DebugTextLog.AddTextToLog($"AI Plan is not possible to perform. Skipping this structure.", DebugTextLogChannel.AILogging);
                    }
                    else
                    {
                        yield return randomInput.Execute(WorldContextInstance, GameplayAnimationInstance);
                    }
                }
            }
        }

        TurnManager.PassTurnToNextPlayer();
    }

    UnitTurnPlan GetBestPersonalPlan(MapMob acting)
    {
        List<UnitTurnPlan> possiblePlans = new List<UnitTurnPlan>();
        possiblePlans.Add(new UnitTurnPlan(new DoesNothingPlayerInput(acting), -100));

        // Get our current movement and possible hurt ranges
        IEnumerable<MapCoordinates> movementRanges = WorldContextInstance.MapHolder.PotentialMoves(acting);
        IEnumerable<MapCoordinates> hurtRanges = movementRanges.SelectMany(mr => WorldContextInstance.MapHolder.PotentialAttacks(acting, mr));

        if (!acting.CanMove)
        {
            movementRanges = new List<MapCoordinates>() { acting.Position };
        }

        if (acting.CanAttack)
        {
            // What opponents are in our hurt range?
            IEnumerable<MapMob> opponentsInRange = WorldContextInstance.MobHolder.ActiveMobs
                .Where(mob => mob.PlayerSideIndex != acting.PlayerSideIndex)
                .Where(mob => hurtRanges.Contains(mob.Position));

            if (opponentsInRange.Any())
            {
                foreach (MapMob inRange in opponentsInRange)
                {
                    IEnumerable<MapCoordinates> engagementTiles = WorldContextInstance.MapHolder.CanHitFrom(acting, inRange.Position)
                        .Intersect(movementRanges);

                    if (!engagementTiles.Any())
                    {
                        continue;
                    }

                    IEnumerable<MapCoordinates> emptyEngagementTiles = engagementTiles.Where(tile => acting.Position == tile || WorldContextInstance.MobHolder.MobOnPoint(tile) == null);

                    if (!emptyEngagementTiles.Any())
                    {
                        continue;
                    }

                    foreach (MapCoordinates validEngagementTile in emptyEngagementTiles)
                    {
                        possiblePlans.Add(new UnitTurnPlan(new AttackWithMobInput(acting, inRange, emptyEngagementTiles.First()), ScoreEngagement(acting, inRange, validEngagementTile)));
                    }
                }
            }
        }        

        // Are there any structures in our movement range?
        IEnumerable<MapStructure> structuresInRange = WorldContextInstance.StructureHolder.ActiveStructures
            .Where(structure => movementRanges.Contains(structure.Position))
            .Where(structure => structure.PlayerSideIndex != acting.PlayerSideIndex)
            .Where(structure => acting.Position == structure.Position || WorldContextInstance.MobHolder.MobOnPoint(structure.Position) == null);

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
            IEnumerable<MapStructure> enemyStructures = WorldContextInstance.StructureHolder.ActiveStructures
                .Where(structure => structure.PlayerSideIndex.HasValue || structure.PlayerSideIndex != TurnManager.CurrentPlayer.PlayerSideIndex)
                .Except(structuresInRange);

            foreach (MapStructure structure in enemyStructures)
            {
                List<MapCoordinates> path = WorldContextInstance.MapHolder.Path(acting, structure.Position);

                if (path == null)
                {
                    continue;
                }

                for (int ii = Mathf.Min(path.Count - 1, acting.MoveRange); ii >= 0; ii--)
                {
                    List<MapCoordinates> pathToSpot = WorldContextInstance.MapHolder.Path(acting, path[ii]);

                    if (pathToSpot == null)
                    {
                        continue;
                    }

                    int distanceModifier = ii;
                    possiblePlans.Add(new UnitTurnPlan(new MoveMobPlayerInput(acting, path[ii]), Mathf.FloorToInt((float)(ScoreCapturing(acting, structure) - (distanceModifier * 4)) * .1f)));

                    break;
                }
            }
        }

        return possiblePlans.OrderByDescending(plan => plan.Score).First();
    }

    decimal ScoreEngagement(MapMob acting, MapMob defending, MapCoordinates attackFrom)
    {
        decimal projectedOutgoingDamage = WorldContextInstance.MobHolder.ProjectedDamages(acting, defending);

        if (projectedOutgoingDamage >= defending.HitPoints)
        {
            return (int)defending.HitPoints + 10;
        }

        if (!WorldContextInstance.MobHolder.CanAttackFromPosition(defending, acting, defending.Position))
        {
            return (int)projectedOutgoingDamage;
        }

        decimal returnDamage = WorldContextInstance.MobHolder.ProjectedDamages(defending, acting, defending.HitPoints - projectedOutgoingDamage);

        return (int)(projectedOutgoingDamage - returnDamage);
    }

    decimal ScoreCapturing(MapMob acting, MapStructure structure)
    {
        decimal projectedCapturePower = acting.CurrentCapturePoints;

        if (projectedCapturePower > structure.CurCapturePoints)
        {
            return structure.CaptureImportance * 10; // TEMPORARY: Finishing a capture is valuable
        }

        return (projectedCapturePower * .2M) + structure.CaptureImportance;
    }

    public void StopAllInputs()
    {
        if (TurnBeingPlayed != null)
        {
            StopCoroutine(TurnBeingPlayed);
        }
    }
}
