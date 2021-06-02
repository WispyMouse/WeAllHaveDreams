using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    public class TileConfiguration : ConfigurationData
    {
        public bool CompletelySolid;
        public string TileName;
        public TileNeighborSpriteSetting[] SpriteSettings = System.Array.Empty<TileNeighborSpriteSetting>();
        public string DefaultSprite;
    }
}
