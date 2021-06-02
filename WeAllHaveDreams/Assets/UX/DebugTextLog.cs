using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextLog : SingletonBase<DebugTextLog>, IDisposable
{
    const int TextLogSize = 8;

    public Text TextLog;
    List<DebugTextEntry> ActiveText = new List<DebugTextEntry>();

    Queue<DebugTextEntry> MessageQueue { get; set; } = new Queue<DebugTextEntry>();

    StreamWriter logFile;

    private void Awake()
    {
        ExplicitlySetSingleton();
        EstablishLogFile();
    }

    private void EstablishLogFile()
    {
        if (logFile != null)
        {
            return;
        }

        logFile = new StreamWriter(Path.Combine(Application.persistentDataPath, $"log.{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}.txt"), true);
    }

    private void Update()
    {
        lock (MessageQueue)
        {
            EstablishLogFile();

            while (MessageQueue.Count > 0)
            {
                DebugTextEntry thisText = MessageQueue.Dequeue();

                if (thisText.Channel == DebugTextLogChannel.Verbose)
                {
                    Singleton.ActiveText.Add(thisText);

                    if (Singleton.ActiveText.Count > TextLogSize)
                    {
                        Singleton.ActiveText.RemoveRange(0, ActiveText.Count - TextLogSize);
                    }

                    Singleton.TextLog.text = string.Join("\n", ActiveText.Select(text => text.Message));
                }

                string logMessage = $"{thisText.Channel}: {thisText.Message}";
                logFile.WriteLine(logMessage);
                Debug.Log(logMessage);
            }
        }
    }

    public static void AddTextToLog(string text, DebugTextLogChannel channel = DebugTextLogChannel.Generic)
    {
        AddTextToLog(new DebugTextEntry(channel, text));
    }

    public static void AddTextToLog(DebugTextEntry toLog)
    {
        lock (Singleton.MessageQueue)
        {
            Singleton.MessageQueue.Enqueue(toLog);
        }
    }

    public void Dispose()
    {
        logFile.Dispose();
    }
}
