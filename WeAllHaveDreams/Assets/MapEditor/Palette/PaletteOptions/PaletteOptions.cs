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

    /// <summary>
    /// Determines what kind of effect this PaletteOption would have on the Input.
    /// This method can be used to determine if an operation should be taken.
    /// </summary>
    /// <param name="worldContextInstance">The current world context.</param>
    /// <param name="toApply">The rule that would be applied to. This method does not modify the Input.</param>
    /// <param name="inputContext">The input context for this consideration.</param>
    /// <returns>An enum indicating what kind of effect this option would have.</returns>
    public virtual OptionPaintApplication DetermineApplication(WorldContext worldContextInstance, MapEditorInput toApply, InputContext inputContext)
    {
        return OptionPaintApplication.Unmodified;
    }

    public abstract void Apply(WorldContext context, MapEditorInput toApply, InputContext inputContext);
}
