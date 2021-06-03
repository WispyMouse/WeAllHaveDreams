using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteTab
{
    public string TabName { get; set; }
    public IEnumerable<PaletteSettings> Settings { get; set; }
    public IEnumerable<PaletteOptions> Options { get; set; }

    public PaletteTab(string tabName, IEnumerable<PaletteSettings> settings)
    {
        this.TabName = tabName;
        this.Settings = settings;
        this.Options = Array.Empty<PaletteOptions>();
    }

    public PaletteTab(string tabName, IEnumerable<PaletteSettings> settings, IEnumerable<PaletteOptions> options)
    {
        this.TabName = tabName;
        this.Settings = settings;
        this.Options = options;
    }
}
