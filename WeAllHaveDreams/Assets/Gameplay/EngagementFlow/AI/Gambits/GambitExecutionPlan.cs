using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;

namespace AI
{
    public class GambitExecutionPlan
    {
        public List<PlayerInput> InputSteps { get; }

        public IEnumerable<GambitCost> PlanCosts { get; }
        public IEnumerable<GambitUtility> ProjectedResults { get; }

        public GambitExecutionPlan(List<PlayerInput> inputSteps, IEnumerable<GambitCost> planCosts, IEnumerable<GambitUtility> projectedResults)
        {
            this.InputSteps = inputSteps;
            this.PlanCosts = planCosts;
            this.ProjectedResults = projectedResults;
        }

        public float ProjectedUtilityValue(AIConfiguration aiConfiguration)
        {
            return ProjectedResults.Sum(res => res.GetValue(aiConfiguration));
        }

        public float ProjectedCostValue(AIConfiguration aiConfiguration)
        {
            return PlanCosts.Sum(res => res.GetMagnitude(aiConfiguration));
        }

        public bool CostsCanBePaid(WorldContext worldContextInstance)
        {
            foreach (GambitCost curCost in PlanCosts)
            {
                if (!curCost.CostCanBePaid(worldContextInstance))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
