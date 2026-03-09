using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GeneralDanceDataManager
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "dances.onlydance");

    private static GeneralSerializables.DanceCollection LoadCollection()
    {
        if (!File.Exists(FilePath))
            return new GeneralSerializables.DanceCollection();

        try
        {
            using var fs = new FileStream(FilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            return (GeneralSerializables.DanceCollection)formatter.Deserialize(fs);
        }
        catch
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Tänze konnten nicht geladen werden.");
            return new GeneralSerializables.DanceCollection();
        }
    }

    private static void SaveCollection(GeneralSerializables.DanceCollection collection)
    {
        try
        {
            using var fs = new FileStream(FilePath, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, collection);
        }
        catch
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Tänze konnten nicht gespeichert werden.");
        }
    }

    public static void SaveDance(GeneralSerializables.DanceData dance)
    {
        var collection = LoadCollection();

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


    public static GeneralSerializables.Step[] LoadDanceSteps(int id)
    {
        var collection = LoadCollection();
        var dance = collection.dances.Find(d => d.id == id);
    
        if (dance == null || dance.data == null) return Array.Empty<GeneralSerializables.Step>();
        return dance.data.ToArray();
    }

    public static GeneralSerializables.DanceData GetDance(int id)
    {
        var collection = LoadCollection();
        return collection.dances.Find(d => d.id == id);
    }
    
    public static List<GeneralSerializables.Dance> GetAllDances()
    {
        var collection = LoadCollection();
        var result = new List<GeneralSerializables.Dance>();

        foreach (var dance in collection.dances)
        {
            result.Add(new GeneralSerializables.Dance
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
            GeneralPopUpManager.ShowInfo("Fehler!", "Daten konnten nicht gelöscht werden.");
        }
    }
}
