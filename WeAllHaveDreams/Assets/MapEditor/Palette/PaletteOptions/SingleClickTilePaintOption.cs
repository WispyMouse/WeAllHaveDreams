using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleClickTilePaintOption : PaletteOptions
{
    public override string OptionsName => "Single Click Tile Paint";
    public override IEnumerable<string> ExclusiveTags => new string[] { PaletteOptionTags.TilePaintClickOperation };

    public override void Apply(WorldContext worldContextInstance, MapEditorInput toApply, InputContext inputContext)
    {
        // Do nothing; use default behavior
    }
}
