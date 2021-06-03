using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTilePaintOption : PaletteOptions
{
    public override string OptionsName => "Drag Paint";

    public override IEnumerable<string> ExclusiveTags => new string[] { PaletteOptionTags.TilePaintClickOperation };
    public override IEnumerable<Type> RelevantPaletteSettings => new Type[] { typeof(TileReplacementAction) };

    public override OptionPaintApplication DetermineApplication(WorldContext worldContextInstance, MapEditorInput toApply, InputContext inputContext)
    {
        if (inputContext.EndClick.HasValue)
        {
            return OptionPaintApplication.FinishInvoke;
        }

        // If we're not done, then continueinvoke; apply this change, but don't move it off the undo stack yet
        return OptionPaintApplication.ContinueInvoke;
    }

    public override void Apply(WorldContext context, MapEditorInput toApply, InputContext inputContext)
    {
        TileReplacementAction tileReplacement = toApply as TileReplacementAction;
        if (tileReplacement == null)
        {
            return;
        }

        tileReplacement.Positions.UnionWith(new MapCoordinates[] { inputContext.CurrentPosition });

        if (!tileReplacement.Removed.ContainsKey(inputContext.CurrentPosition))
        {
            tileReplacement.Removed.Add(inputContext.CurrentPosition, context.MapHolder.GetGameplayTile(inputContext.CurrentPosition)?.TileName);
        }
    }
}
