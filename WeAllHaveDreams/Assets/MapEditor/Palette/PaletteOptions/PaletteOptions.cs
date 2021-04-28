using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PaletteOptions
{
    public abstract string OptionsName { get; }
    public virtual IEnumerable<string> ExclusiveTags
    {
        get
        {
            return Array.Empty<string>();
        }
    }

    public virtual IEnumerable<Type> RelevantPaletteSettings
    {
        get
        {
            return Array.Empty<Type>();
        }
    }

    public bool AppliesToInput(MapEditorInput toCheck)
    {
        return RelevantPaletteSettings.Contains(toCheck.GetType());
    }

    public abstract void Apply(WorldContext context, MapEditorInput toApply);
}
