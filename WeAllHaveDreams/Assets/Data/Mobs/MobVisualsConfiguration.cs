using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    [System.Serializable]
    public class MobVisualsConfiguration : ConfigurationData
    {
        /// <summary>
        /// The color to tint a sprite of a unit is not exhausted. Should be in hex code.
        /// <see cref="MobVisuals.DefaultColor"/>
        /// </summary>
        public string DefaultColor;

        /// <summary>
        /// The color to tint a sprite if a unit is exhausted. Should be in hex code.
        /// <see cref="MobVisuals.ExhaustedColor"/>
        /// </summary>
        public string ExhaustedColor;
    }
}
