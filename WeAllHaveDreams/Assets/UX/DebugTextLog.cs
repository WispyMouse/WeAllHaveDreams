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

    Queue<string> MessageQueue { get; set; } = new Queue<string>();

    private void Awake()
    {
        ExplicitlySetSingleton();
    }

    private void Update()
    {
        lock (MessageQueue)
        {
            while (MessageQueue.Count > 0)
            {
                string thisText = MessageQueue.Dequeue();
                Singleton.ActiveText.Add(thisText);

                if (Singleton.ActiveText.Count > TextLogSize)
                {
                    Singleton.ActiveText.RemoveRange(0, ActiveText.Count - TextLogSize);
                }

                Singleton.TextLog.text = string.Join("\n", ActiveText);
            }
        }
    }

    public static void AddTextToLog(string text, DebugTextLogChannel channel = DebugTextLogChannel.Generic)
    {
        lock (Singleton.MessageQueue)
        {
            Singleton.MessageQueue.Enqueue(text);
        }
        
        Debug.Log(text);
    }
}
