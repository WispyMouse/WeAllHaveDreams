using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputGameplayPhase : MonoBehaviour
{
    const int NumbersCount = 10;
    const int ZeroOverride = 9; // Indicates the index of commands should map to KeyCode.Alpha0
    const int AlphabeticalCount = 25; // Excluding Z

    public virtual IEnumerator EnterPhase()
    {
        yield return new WaitForEndOfFrame();
    }

    public virtual bool TryHandleTileClicked(MapCoordinates position, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;
        return false;
    }

    public virtual bool TryHandleUnitClicked(MapMob mob, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;
        return false;
    }

    public virtual bool TryHandleKeyPress(out InputGameplayPhase nextPhase)
    {
        nextPhase = this;
        return false;
    }

    public virtual bool TryHandleStructureClicked(MapStructure structure, out InputGameplayPhase nextPhase)
    {
        nextPhase = this;
        return false;
    }

    public virtual void EndPhase()
    {

    }

    public virtual void UpdateAfterInput()
    {

    }

    public virtual InputGameplayPhase GetNextPhase()
    {
        return this;
    }

    public virtual bool WaitingForInput => true;

    public virtual bool NextPhasePending => false;

    protected static SortedDictionary<KeyCode, PlayerInput> SortOptionsInToDictionary(List<PlayerInput> inputs)
    {
        SortedDictionary<KeyCode, PlayerInput> inputDictionary = new SortedDictionary<KeyCode, PlayerInput>();

        // The first ten items in the list are mapped from 1-0, the next 26 are mapped from a-z
        // Beyond that, we don't support showing them yet
        for (int ii = 0; ii < NumbersCount + AlphabeticalCount && ii < inputs.Count; ii++)
        {
            PlayerInput curInput = inputs[ii];
            KeyCode baseKeyCode;

            if (ii < NumbersCount)
            {
                // 0 is to the far right on most keyboards, but is before 1 in the list of KeyCodes
                // so wrap around
                if (ii == ZeroOverride)
                {
                    baseKeyCode = KeyCode.Alpha0;
                }
                else
                {
                    baseKeyCode = (KeyCode)((int)KeyCode.Alpha1 + ii);
                }
            }
            else
            {
                int adjustedOffset = ii - NumbersCount;
                baseKeyCode = (KeyCode)((int)KeyCode.A + adjustedOffset);
            }

            DebugTextLog.AddTextToLog($"Setting {baseKeyCode.ToString()} to {curInput.LongTitle}", DebugTextLogChannel.Verbose);
            inputDictionary.Add(baseKeyCode, curInput);
        }

        if (inputs.Count > NumbersCount + AlphabeticalCount)
        {
            DebugTextLog.AddTextToLog($"Only able to support {NumbersCount + AlphabeticalCount} inputs, but there were {inputs.Count}", DebugTextLogChannel.DebugOperationInputInstructions);
        }

        return inputDictionary;
    }
}
