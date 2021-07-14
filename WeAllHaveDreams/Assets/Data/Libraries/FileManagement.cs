using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FileManagement : SingletonBase<FileManagement>
{
    public const string MapFolderPathRelative = "Resources/Maps";
    public static string MapFolderPath
    {
        get
        {
            return Path.Combine(Application.dataPath, MapFolderPathRelative).Replace("\\", "/");
        }
    }

    public const string MapFileSuffix = ".json";
    static string MapFileSearch
    {
        get
        {
            return $"*{MapFileSuffix}";
        }
    }

    public static IEnumerator SaveRealm(Realm toSave)
    {
        string serializedRealm = Newtonsoft.Json.JsonConvert.SerializeObject(toSave);
        string savePath = Path.Combine(MapFolderPath, $"{toSave.Name}{MapFileSuffix}");

        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            byte[] dataBuffer = Encoding.UTF8.GetBytes(serializedRealm);
            yield return ThreadDoctor.YieldTask(stream.WriteAsync(dataBuffer, 0, dataBuffer.Length));
        }

        DebugTextLog.AddTextToLog($"Realm saved", DebugTextLogChannel.MapEditorOperations);
        GameplayMapBootup.WIPRealm = toSave;
    }

    public static async Task<IEnumerable<Realm>> GetAllRealms()
    {
        try
        {
            List<Realm> realms = new List<Realm>();

            foreach (string filePath in Directory.GetFiles(MapFolderPath, MapFileSearch, SearchOption.AllDirectories))
            {
                string sanitizedFilePath = filePath.Replace("\\", "/");

                // HACK TODO: This just gets the first one it finds and calls it good
                string fileText;

                using (var reader = File.OpenText(sanitizedFilePath))
                {
                    fileText = await reader.ReadToEndAsync();
                }

                Realm thisRealm = JsonConvert.DeserializeObject<Realm>(fileText);
                DebugTextLog.AddTextToLog($"Realm loaded with {thisRealm.RealmCoordinates.Count()} coordinates");
                realms.Add(thisRealm);
            }

            return realms;
        }
        catch (Exception e)
        {
            DebugTextLog.AddTextToLog(e.Message, DebugTextLogChannel.RuntimeError);
            throw e;
        }
    }
}
