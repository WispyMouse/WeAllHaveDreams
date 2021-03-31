using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapEditorInput
{
    public abstract void Invoke(WorldContext worldContextInstance);
    public abstract void Undo(WorldContext worldContextInstance);
}
