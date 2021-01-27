using Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ConfigurationLoadingEntrypoint : SingletonBase<ConfigurationLoadingEntrypoint>
{
    const string ConfigurationFolderPathRelative = "Resources/Configurations";
    public static string ConfigurationFolderPath
    {
        get
        {
            return Path.Combine(Application.dataPath, ConfigurationFolderPathRelative).Replace("\\", "/");
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
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        Singleton.configurationData = new HashSet<ConfigurationData>();

        DebugTextLog.AddTextToLog($"The configuration folder is at: {ConfigurationFolderPath}");

        foreach (string filePath in Directory.GetFiles(ConfigurationFolderPath, ConfigurationFileSearch, SearchOption.AllDirectories))
        {
            string sanitizedFilePath = filePath.Replace("\\", "/");

            DebugTextLog.AddTextToLog($"Found {sanitizedFilePath}");
            try
            {
                string fileText;

                using (var reader = File.OpenText(sanitizedFilePath))
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
                ConfigurationData specifiedData = (ConfigurationData)JsonConvert.DeserializeObject(fileText, specifiedType);

                Singleton.configurationData.Add(specifiedData);
                DebugTextLog.AddTextToLog($"Processed!");

                string message = specifiedData.GetConfigurationShortReport();

                if (!string.IsNullOrEmpty(message))
                {
                    DebugTextLog.AddTextToLog(message);
                }
            }
            catch (Exception e)
            {
                DebugTextLog.AddTextToLog($"Unable to process file! {e.Message}, {e.InnerException?.Message}");
            }
        }

        await MobLibrary.LoadMobsFromConfiguration();
    }

    public static IEnumerable<T> GetConfigurationData<T>() where T : ConfigurationData, new()
    {
        IEnumerable<ConfigurationData> matchingData = Singleton.configurationData.Where(data => data is T);

        if (!matchingData.Any())
        {
            DebugTextLog.AddTextToLog($"Configuration data did not have {typeof(T).ToString()}.", DebugTextLogChannel.ConfigurationError);
            return null;
        }

        return matchingData.Select(md => md as T);
    }
}
