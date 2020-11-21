using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextLog : SingletonBase<DebugTextLog>
{
    public Text TextLog;

    public static void AddTextToLog(string text)
    {
        Singleton.TextLog.text += $"\n{text}";
    }
}
