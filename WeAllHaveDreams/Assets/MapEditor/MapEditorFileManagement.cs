using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapEditorFileManagement : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();
    public bool MapHasBeenSavedBefore { get; set; } = false;

    // HACK Temporary: This is where we're storing the name of existing realms for this commit, going to change soon
    public static string CurrentMapName { get; set; }

    public async Task<Realm> SaveNewRealm()
    {
        MapHasBeenSavedBefore = true;

        Realm toSave = GenerateRealm();

        if (!string.IsNullOrWhiteSpace(CurrentMapName))
        {
            toSave.Name = CurrentMapName;
        }

        string serializedRealm = Newtonsoft.Json.JsonConvert.SerializeObject(toSave);
        string savePath = Path.Combine(MapBootup.MapFolderPath, $"{CurrentMapName}{MapBootup.MapFileSuffix}");

        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            byte[] dataBuffer = Encoding.UTF8.GetBytes(serializedRealm);
            await stream.WriteAsync(dataBuffer, 0, dataBuffer.Length);
        }

        DebugTextLog.AddTextToLog($"Realm saved", DebugTextLogChannel.MapEditorOperations);

        return toSave;
    }

    public async Task<Realm> SaveExistingRealm()
    {
        MapHasBeenSavedBefore = true;

        Realm toSave = GenerateRealm();

        if (!string.IsNullOrWhiteSpace(CurrentMapName))
        {
            toSave.Name = CurrentMapName;
        }

        string serializedRealm = Newtonsoft.Json.JsonConvert.SerializeObject(toSave);
        string savePath = Path.Combine(MapBootup.MapFolderPath, $"{toSave.Name}{MapBootup.MapFileSuffix}");

        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            byte[] dataBuffer = Encoding.UTF8.GetBytes(serializedRealm);
            await stream.WriteAsync(dataBuffer, 0, dataBuffer.Length);
        }

        DebugTextLog.AddTextToLog($"Realm saved", DebugTextLogChannel.MapEditorOperations);

        return toSave;
    }

    Realm GenerateRealm()
    {
        Realm newRealm = new Realm();

        List<RealmCoordinate> realmCoordinates = new List<RealmCoordinate>();

        foreach (Vector3Int position in WorldContextInstance.MapHolder.GetAllTiles())
        {
            GameplayTile tile = WorldContextInstance.MapHolder.GetGameplayTile(position);
            realmCoordinates.Add(new RealmCoordinate() { Position = position, Tile = tile.name });
        }

        newRealm.RealmCoordinates = realmCoordinates;

        return newRealm;
    }
}
