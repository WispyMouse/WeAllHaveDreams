using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    [System.Serializable]
    public class AIConfiguration : ConfigurationData
    {
        public AIConfiguration() : base()
        {
            
        }

        /// <summary>
        /// When a Gambit would exhaust a unit, this value is multiplied by their resource cost * current health percentage to determine the score of the action.
        /// </summary>
        public float UnitExhaustionCostMultiplier;

        /// <summary>
        /// When a unit would be defeated, this value is multiplied by their resource cost to determine the score of the action. This value is added to UnitDamage considerations.
        /// </summary>
        public float UnitDefeatMultiplier;

        /// <summary>
        /// When a unit would be damaged, this value is multiplied by their resource cost to determine the score of the action.
        /// </summary>
        public float UnitDamageMultiplier;
    }
}