using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    [System.Serializable]
    public class StructureConfiguration : ConfigurationData
    {
        public string Name;

        public string DevelopmentName;

        public string Appearance;

        public IEnumerable<StructureConfigurationAbility> Abilities;

        public IEnumerable<StructureConfigurationAbility> GetSaturatedAbilities()
        {
            if (Abilities == null)
            {
                return new List<StructureConfigurationAbility>();
            }

            List<StructureConfigurationAbility> saturatedAbilities = new List<StructureConfigurationAbility>();

            foreach (StructureConfigurationAbility curAbility in Abilities)
            {
                Type abilityType = Type.GetType(curAbility.AbilityName);
                StructureConfigurationAbility boxedType = Activator.CreateInstance(abilityType) as StructureConfigurationAbility;
                boxedType.Load(curAbility);
                saturatedAbilities.Add(boxedType);
            }

            return saturatedAbilities;
        }
    }
}
