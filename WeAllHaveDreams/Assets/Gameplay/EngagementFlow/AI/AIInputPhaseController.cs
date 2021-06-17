using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{
    public class AIInputPhaseController : MonoBehaviour
    {
        // This is where all of the Gambits that the AI will consider are set, currently
        IEnumerable<Gambit> Gambits => new List<Gambit>
        {
            new AttackInRangeMob()
        };

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
            List<GambitExecutionPlan> executionPlans = GetAllGambitPlans().ToList();

            if (executionPlans.Count == 0)
            {
                DebugTextLog.AddTextToLog("AI didn't make any plans with any gambits.", DebugTextLogChannel.AILogging);
                TurnManager.PassTurnToNextPlayer();
                yield break;
            }

            while (executionPlans.Any())
            {
                GambitExecutionPlan curPlan = ChoosePlan(executionPlans);

                if (curPlan == null)
                {
                    DebugTextLog.AddTextToLog("AI didn't choose any plans. Ending turn.", DebugTextLogChannel.AILogging);
                    break;
                }

                executionPlans.Remove(curPlan);

                if (!curPlan.CostsCanBePaid(WorldContextInstance))
                {
                    DebugTextLog.AddTextToLog($"AI Plan can not pay costs. Moving to next plan.", DebugTextLogChannel.AILogging);
                    continue;
                }

                foreach (PlayerInput curInput in curPlan.InputSteps)
                {
                    if (!curInput.IsPossible(WorldContextInstance))
                    {
                        DebugTextLog.AddTextToLog($"AI Plan is not possible to perform. Moving to next plan.", DebugTextLogChannel.AILogging);
                        break;
                    }

                    yield return curInput.Execute(WorldContextInstance, GameplayAnimationInstance);
                }
            }

            TurnManager.PassTurnToNextPlayer();
        }

        GambitExecutionPlan ChoosePlan(IEnumerable<GambitExecutionPlan> plans)
        {
            List<Tuple<GambitExecutionPlan, float>> consideredPlans = new List<Tuple<GambitExecutionPlan, float>>();

            foreach (GambitExecutionPlan plan in plans)
            {
                float utility = plan.ProjectedUtilityValue(AISettings);
                float cost = plan.ProjectedCostValue(AISettings);

                consideredPlans.Add(new Tuple<GambitExecutionPlan, float>(plan, utility - cost));
            }

            // TODO For more randomness, take less than just the maximum; this'll only pick the singular highest utility skill as is
            float planUtilityThreshhold = consideredPlans.Max(plan => plan.Item2);
            IEnumerable<GambitExecutionPlan> plansOverThreshhold = consideredPlans.Where(plan => plan.Item2 >= planUtilityThreshhold).Select(plan => plan.Item1);

            int randomPlan = UnityEngine.Random.Range(0, plansOverThreshhold.Count());
            return plansOverThreshhold.ToList()[randomPlan];
        }

        IEnumerable<GambitExecutionPlan> GetAllGambitPlans()
        {
            return Gambits.SelectMany(curGambit => curGambit.GetPlans(WorldContextInstance)).ToList();
        }

        public void StopAllInputs()
        {
            if (TurnBeingPlayed != null)
            {
                StopCoroutine(TurnBeingPlayed);
            }
        }
    }

}
