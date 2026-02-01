using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GeneralUserDataManager
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "data.onlydance");

    public static void SaveData(string email, string password)
    {
        try
        {
            var data = new GeneralSerializables.User(email, password);
            var FilePath = Path.Combine(Application.persistentDataPath, "data.onlydance");
            using var fs = new FileStream(FilePath, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, data);
        }
        catch
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Daten konnten nicht gespeichert werden.");
        }
    }

    public static GeneralSerializables.User LoadData()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                GeneralPopUpManager.ShowInfo("Fehler!", "Daten konnten nicht gefunden werden.");
                return null;
            }

            using var fs = new FileStream(FilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            var data = (GeneralSerializables.User)formatter.Deserialize(fs);
            return data;
        }
        catch
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Daten konnten nicht gefunden werden.");
            return null;
        }
    }

    public static void DeleteData()
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

    public static GeneralSerializables.User LoadDataAuthentication()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                return null;
            }

            using var fs = new FileStream(FilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            var data = (GeneralSerializables.User)formatter.Deserialize(fs);
            return data;
        }
        catch
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Daten konnten nicht gefunden werden.");
            return null;
        }
    }
}