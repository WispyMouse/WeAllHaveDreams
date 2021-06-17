using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    public class Gambit
    {
        /// <summary>
        /// Gets all plans this Gambit can calculate given the current worldstate.
        /// You can then execute these plans by following their set of PlayerInputs.
        /// Can be overridden by Gambits for more complex plans.
        /// </summary>
        /// <param name="worldContextInstance">The current world context.</param>
        /// <returns>All plans calculated by this Gambit.</returns>
        public virtual IEnumerable<GambitExecutionPlan> GetPlans(WorldContext worldContextInstance)
        {
            List<GambitExecutionPlan> plans = new List<GambitExecutionPlan>();

            foreach (MapMob curMob in worldContextInstance.MobHolder.MobsOnTeam(TurnManager.CurrentPlayer).Where(mob => !mob.IsExhausted))
            {
                plans.AddRange(GetPlansForMob(curMob, worldContextInstance));
            }

            return plans;
        }

        /// <summary>
        /// Gets all plans that a particular Mob can perform.
        /// This provides a possible hook for overriding classes to hook in to. Optional to implement.
        /// </summary>
        /// <param name="thisMob">The mob to calculate plans for.</param>
        /// <param name="worldContextInstance">The current world context.</param>
        /// <returns>All plans calculated by this Gambit for this Mob.</returns>
        protected virtual IEnumerable<GambitExecutionPlan> GetPlansForMob(MapMob thisMob, WorldContext worldContextInstance)
        {
            return Array.Empty<GambitExecutionPlan>();
        }
    }
}
