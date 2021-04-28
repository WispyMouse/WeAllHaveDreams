using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaletteOptionsCollection
{
    List<PaletteOptions> selectedOptions { get; set; } = new List<PaletteOptions>();

    public void AddPaletteOption(PaletteOptions toAdd)
    {
        // If we've hit this same operation, then we should stop what we're doing and add nothing! It's already in this collection
        if (Contains(toAdd))
        {
            return;
        }

        // If this incoming option has an exclusive tag that something else has, remove the existing options with that tag
        for (int ii = selectedOptions.Count - 1; ii >= 0; ii--)
        {
            PaletteOptions optionAtIndex = selectedOptions[ii];

            if (optionAtIndex.ExclusiveTags.Any(tag => toAdd.ExclusiveTags.Contains(tag)))
            {
                selectedOptions.RemoveAt(ii);
            }
        }

        selectedOptions.Add(toAdd);
    }

    public OptionPaintApplication DetermineApplication(WorldContext worldContextInstance, MapEditorInput toApplyTo, InputContext inputContext)
    {
        OptionPaintApplication application = OptionPaintApplication.Unmodified;

        foreach (PaletteOptions option in selectedOptions.Where(option => option.AppliesToInput(toApplyTo)))
        {
            application = option.DetermineApplication(worldContextInstance, toApplyTo, inputContext);

            if (application != OptionPaintApplication.Unmodified)
            {
                return application;
            }
        }

        return application;
    }

    public void ApplyOptions(WorldContext worldContextInstance, MapEditorInput toApplyTo, InputContext inputContext)
    {
        OptionPaintApplication application = OptionPaintApplication.Unmodified;

        foreach (PaletteOptions curOption in selectedOptions.Where(option => option.AppliesToInput(toApplyTo)))
        {
            application = curOption.DetermineApplication(worldContextInstance, toApplyTo, inputContext);

            if (application == OptionPaintApplication.Invoke)
            {
                curOption.Apply(worldContextInstance, toApplyTo, inputContext);
                break;
            }
        }
    }

    public bool Contains(PaletteOptions toCheck)
    {
        return selectedOptions.Any(option => option.OptionsName == toCheck.OptionsName);
    }

    public void Remove(PaletteOptions toRemove)
    {
        selectedOptions.Remove(toRemove);
    }
}
