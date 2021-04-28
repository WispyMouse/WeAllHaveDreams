using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The classifications of effects your active <see cref="PaletteOptionsCollection"/> can have on an <see cref="MapEditorInput"/>.
/// This refers to whether the input should be "invoked".
/// </summary>
public enum OptionPaintApplication
{
    /// <summary>
    /// Indicates that nothing in the PaletteOptionsCollection had an affect on the MapEditorInput.
    /// If nothing modifies a paint operation, then we can Invoke the MapEditorInput on mouse down.
    /// </summary>
    Unmodified,

    /// <summary>
    /// Indicates that a PaletteOptions recognizes this input, and says it should not be invoked as-is.
    /// If this is returned, then the input is ignored / unhandled.
    /// </summary>
    DoNotInvoke,

    /// <summary>
    /// Indicates that given this context, the input should be invoked.
    /// </summary>
    Invoke
}
