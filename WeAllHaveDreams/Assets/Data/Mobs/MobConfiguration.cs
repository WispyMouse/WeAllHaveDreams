﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    [System.Serializable]
    public class MobConfiguration : ConfigurationData
    {
        /// <summary>
        /// The name of the Mob this Configuration is for. Displayed to players.
        /// </summary>
        public string Name;

        /// <summary>
        /// The internal name for this Mob. This is used for placing them in maps and referring to them internally.
        /// </summary>
        public string DevelopmentName;

        /// <summary>
        /// Generic Resource Cost for this Mob. Used by the AI to inform some decisions and for generic shops. Not everything is buyable, but everything should have a ResourceCost.
        /// </summary>
        public int ResourceCost;

        /// <summary>
        /// The movement range of this Mob. How many tiles this Mob can move in an open field.
        /// </summary>
        public int MoveRange;

        /// <summary>
        /// The sight range of this Mob. How far in tiles the Mob can see across an open field.
        /// </summary>
        public int SightRange;

        /// <summary>
        /// The attack range of this Mob. The Mob can attack units that are this many tiles away in orthogonal distance.
        /// </summary>
        public int AttackRange;

        /// <summary>
        /// The damage output of this Mob. Take the Mob's current Health, multiply it by this number, and their attacks do this much damage. There are other factors possible.
        /// "1" means that at 10 Health, this Mob would do 10 damage.
        /// </summary>
        public decimal DamageOutputRatio;

        /// <summary>
        /// The damage reduction of this Mob. Take the total damage they were going to take, and multiply it by this number.
        /// "1" means no damage reduction, ".1" means reduce damage by 90%, "1.1" means take 10% more damage.
        /// </summary>
        public decimal DamageReductionRatio;

        /// <summary>
        /// What sprite set does this Mob load in?
        /// This is a temporary, hacky solution to having different visuals for Mobs.
        /// </summary>
        public string Appearance;

        public MobConfiguration() : base()
        {
        }

        public override string GetConfigurationShortReport()
        {
            return $"Name: {Name}";
        }
    }
}