using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDragTilePaintOption : PaletteOptions
{
    public override string OptionsName => "Area Drag";
    public override IEnumerable<string> ExclusiveTags => new string[] { PaletteOptionTags.TilePaintClickOperation };
    public override IEnumerable<Type> RelevantPaletteSettings => new Type[] { typeof(TileReplacementAction) };

    public override OptionPaintApplication DetermineApplication(WorldContext worldContextInstance, MapEditorInput toApply, InputContext inputContext)
    {
        if (!inputContext.EndClick.HasValue)
        {
            return OptionPaintApplication.DoNotInvoke;
        }

        return OptionPaintApplication.Invoke;
    }

    public override void Apply(WorldContext context, MapEditorInput toApply, InputContext inputContext)
    {
        TileReplacementAction tileReplacement = toApply as TileReplacementAction;
        if (tileReplacement == null)
        {
            return;
        }

        HashSet<MapCoordinates> coordinates = new HashSet<MapCoordinates>();

        int left = Math.Min(inputContext.StartClick.X, inputContext.EndClick.Value.X);
        int right = Math.Max(inputContext.StartClick.X, inputContext.EndClick.Value.X);
        int bottom = Math.Min(inputContext.StartClick.Y, inputContext.EndClick.Value.Y);
        int top = Math.Max(inputContext.StartClick.Y, inputContext.EndClick.Value.Y);

        for (int xx = left; xx <= right; xx++)
        {
            for (int yy = bottom; yy <= top; yy++)
            {
                coordinates.Add(new MapCoordinates(xx, yy));
            }
        }

        tileReplacement.Positions = coordinates;
        tileReplacement.Removed = tileReplacement.GetRemovedTiles(context);
    }
}
