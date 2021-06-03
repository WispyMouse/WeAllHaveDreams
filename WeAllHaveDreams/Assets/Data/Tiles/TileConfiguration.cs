using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    public class TileConfiguration : ConfigurationData
    {
        public string TileName;

        public IEnumerable<MovementCostAttribute> MovementCostAttributes = System.Array.Empty<MovementCostAttribute>();
        public IEnumerable<DefensiveAttributes> Defenses = Array.Empty<DefensiveAttributes>();

        public bool ObstructsVision;

        public TileNeighborSpriteSetting[] SpriteSettings = System.Array.Empty<TileNeighborSpriteSetting>();
        public string DefaultSprite;
    }
}
