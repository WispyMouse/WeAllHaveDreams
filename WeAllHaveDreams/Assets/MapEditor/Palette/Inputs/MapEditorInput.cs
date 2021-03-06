﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all inputs in the Map Editor.
/// An input is derived from <see cref="PaletteSettings"/>, and then <see cref="Invoke(WorldContext)"/> is run on to the provided <see cref="WorldContext"/>.
/// Many constructors for inherited classes use <see cref="WorldContext"/>, but the WorldContext should never be stored.
/// </summary>
public abstract class MapEditorInput
{
    /// <summary>
    /// Applies this Input to the WorldContext.
    /// </summary>
    /// <param name="worldContextInstance">The current WorldContext.</param>
    public abstract void Invoke(WorldContext worldContextInstance);

    /// <summary>
    /// Undoes this Input, returning the WorldContext to the state it was before <see cref="Invoke(WorldContext)"/> was run.
    /// </summary>
    /// <param name="worldContextInstance">The current WorldContext.</param>
    public abstract void Undo(WorldContext worldContextInstance);
}
