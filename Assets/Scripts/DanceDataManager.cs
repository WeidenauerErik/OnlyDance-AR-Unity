using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class DanceData
{
    public int id;
    public string name;
    public int BPM;
    public List<Step> data;
}

[Serializable]
public class DanceCollection
{
    public List<DanceData> dances = new List<DanceData>();
}

public static class DanceDataManager
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "dances.onlydance");

    private static DanceCollection LoadCollection()
    {
        if (!File.Exists(FilePath))
            return new DanceCollection();

        try
        {
            using var fs = new FileStream(FilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            return (DanceCollection)formatter.Deserialize(fs);
        }
        catch
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Tänze konnten nicht geladen werden.");
            return new DanceCollection();
        }
    }

    private static void SaveCollection(DanceCollection collection)
    {
        try
        {
            using var fs = new FileStream(FilePath, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, collection);
        }
        catch
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Tänze konnten nicht gespeichert werden.");
        }
    }

    public static void SaveDance(DanceData dance)
    {
        Debug.Log(dance.id);
        var collection = LoadCollection();
        Debug.Log(collection.dances);

        int index = collection.dances.FindIndex(d => d.id == dance.id);
        if (index >= 0)
        {
            collection.dances[index] = dance;
        }
        else
        {
            if (dance.id <= 0)
                dance.id = collection.dances.Count > 0 ? collection.dances[^1].id + 1 : 1;

            collection.dances.Add(dance);
        }

        SaveCollection(collection);
    }


public static Step[] LoadDanceSteps(int id)
{
    var collection = LoadCollection();
    var dance = collection.dances.Find(d => d.id == id);

    if (dance == null || dance.data == null) return Array.Empty<Step>();
    return dance.data.ToArray();
}

    
    public static List<Dance> GetAllDances()
    {
        var collection = LoadCollection();
        var result = new List<Dance>();

        foreach (var dance in collection.dances)
        {
            result.Add(new Dance
            {
                id = dance.id,
                name = dance.name
            });
        }

        return result;
    }

    public static void DeleteDance(int id)
    {
        var collection = LoadCollection();
        int index = collection.dances.FindIndex(d => d.id == id);
        if (index >= 0)
        {
            collection.dances.RemoveAt(index);
            SaveCollection(collection);
        }
    }
    public static void DeleteAllDances()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Daten konnten nicht gelöscht werden.");
        }
    }
}
