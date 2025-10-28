using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class UserData
{
    public string email;
    public string password;

    public UserData(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
}

public static class DataManagerGeneral
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "data.onlydance");

    public static void SaveData(string email, string password)
    {
        try
        {
            var data = new UserData(email, password);

            using var fs = new FileStream(FilePath, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, data);
        }
        catch (Exception ex)
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Daten konnten nicht gespeichert werden.");
        }
    }

    public static UserData LoadData()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                PopUpManagerGeneral.ShowInfo("Fehler!", "Daten konnten nicht gefunden werden.");
                return null;
            }

            using var fs = new FileStream(FilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            var data = (UserData)formatter.Deserialize(fs);
            return data;
        }
        catch (Exception ex)
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Daten konnten nicht gefunden werden.");
            return null;
        }
    }

    public static void DeleteData()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch (Exception ex)
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Daten konnten nicht gelöscht werden.");
        }
    }
}