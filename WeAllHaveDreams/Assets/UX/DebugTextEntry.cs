using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DebugTextEntry
{
    public DebugTextLogChannel Channel { get; set; }
    public string Message { get; set; }

    public DebugTextEntry(DebugTextLogChannel channel, string message)
    {
        this.Channel = channel;
        this.Message = message;
    }
}
