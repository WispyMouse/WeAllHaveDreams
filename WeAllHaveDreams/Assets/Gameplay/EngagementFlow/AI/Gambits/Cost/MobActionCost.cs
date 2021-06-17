using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;

namespace AI
{
    public class MobActionCost : GambitCost
    {
        MapMob checkedMob { get; set; }
        bool checkMove { get; set; }
        bool checkAttack { get; set; }
        bool checkCapture { get; set; }

        public MobActionCost(MapMob toCheck, bool checkMove = false, bool checkAttack = false, bool checkCapture = false)
        {
            this.checkedMob = toCheck;
            this.checkMove = checkMove;
            this.checkAttack = checkAttack;
            this.checkCapture = checkCapture;
        }

        public override bool CostCanBePaid(WorldContext worldContextInstance)
        {
            if (checkedMob == null)
            {
                DebugTextLog.AddTextToLog("Could not pay MobActionCheck cost, Mob was null", DebugTextLogChannel.AILogging);
                return false;
            }

            if (checkMove && !checkedMob.CanMove)
            {
                return false;
            }

            if (checkAttack && !checkedMob.CanAttack)
            {
                return false;
            }

            if (checkCapture && !checkedMob.CanCapture)
            {
                return false;
            }

            return true;
        }

        public override float GetMagnitude(AIConfiguration aiConfiguration)
        {
            return (float)checkedMob.CurrentAttackPower * aiConfiguration.UnitExhaustionCostMultiplier;
        }
    }
}
