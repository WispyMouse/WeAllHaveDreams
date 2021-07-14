using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    public class AttackInRangeMob : Gambit
    {
        protected override IEnumerable<GambitExecutionPlan> GetPlansForMob(MapMob thisMob, WorldContext worldContextInstance)
        {
            // If this mob cannot attack, then this Gambit doesn't apply
            if (!thisMob.CanAttack)
            {
                return Array.Empty<GambitExecutionPlan>();
            }

            List<GambitExecutionPlan> plans = new List<GambitExecutionPlan>();

            // Select our Mob, and then find every tile this mob could move to if they can move
            // From each of those tiles, find each attack this mob can make against each enemy
            IEnumerable<MapCoordinates> movementRanges = movementRanges = new List<MapCoordinates>() { thisMob.Position };
            if (thisMob.CanMove)
            {
                movementRanges = worldContextInstance.MapHolder.PotentialMoves(thisMob);
            }

            foreach (MapCoordinates standingCoordinate in movementRanges)
            {
                IEnumerable<MapCoordinates> hurtRanges = worldContextInstance.MapHolder.PotentialAttacks(thisMob, standingCoordinate);

                foreach (MapCoordinates hurtCoordinate in hurtRanges)
                {
                    MapMob standingMob = worldContextInstance.MobHolder.MobOnPoint(hurtCoordinate);

                    if (standingMob == null)
                    {
                        continue;
                    }

                    // TODO: This should ask if they're enemies, rather than exactly on the same side
                    if (standingMob.MyPlayerSide == thisMob.MyPlayerSide)
                    {
                        continue;
                    }

                    AttackWithMobInput thisAttackInput = new AttackWithMobInput(thisMob, standingMob, standingCoordinate);

                    GambitExecutionPlan plan = new GambitExecutionPlan(
                        new List<PlayerInput>() { thisAttackInput },
                        new List<GambitCost>()
                        {
                            new MobActionCost(thisMob, standingCoordinate, checkMove: standingCoordinate != thisMob.Position, checkAttack: true)
                        },
                        new List<GambitUtility>()
                        {
                            new MobDamaged(thisMob.MyPlayerSide, thisMob, thisMob.HitPoints, thisAttackInput.ProjectedAttackerHitpoints(worldContextInstance)),
                            new MobDamaged(standingMob.MyPlayerSide, thisMob, thisMob.HitPoints, thisAttackInput.ProjectedAttackerHitpoints(worldContextInstance))
                        }
                        );

                    plans.Add(plan);
                }
            }

            return plans;
        }
    }
}
