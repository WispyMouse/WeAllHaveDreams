using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;

namespace AI
{
    public class MobDamaged : GambitUtility
    {
        bool isEnemy { get; set; }
        MapMob mobChecked { get; set; }
        decimal initialHealth { get; set; }
        decimal finalHealth { get; set; }

        public MobDamaged(PlayerSide myPlayerSide, MapMob toCheck, decimal initialHealth, decimal finalHealth)
        {
            this.mobChecked = toCheck;
            this.initialHealth = initialHealth;
            this.finalHealth = finalHealth;

            this.isEnemy = toCheck.MyPlayerSide != myPlayerSide;
        }

        public override float GetValue(AIConfiguration aiConfiguration)
        {
            decimal damageDifferential = initialHealth - finalHealth;
            float damagePercentage = (float)damageDifferential / 10f;
            float totalPoints = aiConfiguration.UnitDamageMultiplier * damagePercentage * mobChecked.ResourceCost;

            if (finalHealth == 0)
            {
                totalPoints += aiConfiguration.UnitDefeatMultiplier * mobChecked.ResourceCost;
            }

            // If this is on our own unit, then invert the score
            if (!isEnemy)
            {
                totalPoints *= -1f;
            }

            return totalPoints;
        }
    }
}
