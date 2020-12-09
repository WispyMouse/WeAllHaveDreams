﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ConfigurationLoadingEntrypoint : SingletonBase<ConfigurationLoadingEntrypoint>
{
    // should start with a /
    const string ConfigurationFolderPathRelative = "/Resources";
    public static string ConfigurationFolderPath
    {
        get
        {
            return $"{Application.dataPath}{ConfigurationFolderPathRelative}";
        }
    }

    const string ConfigurationFileSuffix = ".json";
    static string ConfigurationFileSearch
    {
        get
        {
            return $"*{ConfigurationFileSuffix}";
        }
    }

    HashSet<ConfigurationData> configurationData;

    public static async Task LoadAllConfigurationData()
    {
        Debug.LogWarning(typeof(Configuration.FogVisibilityConfigurations).FullName);

        Singleton.configurationData = new HashSet<ConfigurationData>();

        DebugTextLog.AddTextToLog($"The configuration folder is at: {ConfigurationFolderPath}");

        foreach (string filePath in Directory.GetFiles(ConfigurationFolderPath, ConfigurationFileSearch, SearchOption.AllDirectories))
        {
            DebugTextLog.AddTextToLog($"Found {filePath}");
            try
            {
                string fileText;

                using (var reader = File.OpenText(filePath))
                {
                    fileText = await reader.ReadToEndAsync();
                }

                DebugTextLog.AddTextToLog("Loaded text");
                Debug.Log(fileText);

                DebugTextLog.AddTextToLog("Converting to ConfigurationData");
                ConfigurationData baseData = JsonUtility.FromJson<ConfigurationData>(fileText);

                DebugTextLog.AddTextToLog($"Getting type ({baseData.ConfigurationType})");
                Type specifiedType = Type.GetType(baseData.ConfigurationType, true, true);

                DebugTextLog.AddTextToLog($"Converting to {specifiedType}");
                ConfigurationData specifiedData = JsonUtility.FromJson(fileText, specifiedType) as ConfigurationData;

                Singleton.configurationData.Add(specifiedData);
                DebugTextLog.AddTextToLog($"Processed!");
            }
            catch (Exception e)
            {
                DebugTextLog.AddTextToLog($"Unable to process file! {e.Message}, {e.InnerException?.Message}");
            }
        }
    }

    public static T GetConfigurationData<T>() where T : ConfigurationData, new()
    {
        ConfigurationData matchingData = Singleton.configurationData.FirstOrDefault(data => data is T);

        if (matchingData == null)
        {
            DebugTextLog.AddTextToLog($"Configuration data did not have {typeof(T).ToString()}. Returning a default.");
            matchingData = new T();
            Singleton.configurationData.Add(matchingData);
        }

        return matchingData as T;
    }
}
