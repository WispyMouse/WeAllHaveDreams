using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextLog : SingletonBase<DebugTextLog>
{
    const int TextLogSize = 8;

    public Text TextLog;
    List<string> ActiveText = new List<string>();

    public static void AddTextToLog(string text, DebugTextLogChannel channel = DebugTextLogChannel.Generic)
    {
        Singleton.ActiveText.Add(text);

        if (Singleton.ActiveText.Count > TextLogSize)
        {
            Singleton.ActiveText.RemoveRange(0, Singleton.ActiveText.Count - TextLogSize);
        }

        Singleton.TextLog.text = string.Join("\n", Singleton.ActiveText);
        Debug.Log(text);
    }
}
