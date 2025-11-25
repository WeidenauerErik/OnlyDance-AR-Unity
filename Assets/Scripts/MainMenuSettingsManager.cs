using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuSettingsManager : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView, MonoBehaviour coroutineOwner)
    {
        mainView.Clear();
        mainView.Add(MainMenu.CreateHeading("Einstellungen"));

        var data = GeneralUserDataManager.LoadData();
        var emailLabel = new Label { text = data.email };
        emailLabel.AddToClassList("text-medium");
		emailLabel.AddToClassList("email-label");

        var emailContainer = new VisualElement();
        emailContainer.AddToClassList("settings-emailContainer");
        emailContainer.AddToClassList("settings-container");
        emailContainer.Add(emailLabel);
        mainView.Add(emailContainer);

        var changePassword = new Button { text = "Passwort ändern" };
        changePassword.AddToClassList("button");
        changePassword.AddToClassList("settings-container");
        changePassword.clicked += () =>
        {
            GeneralPopUpManager.ShowChangePassword((oldPwd, newPwd, confirmPwd) =>
            {
                GeneralPopUpManager.ResetInstance();
                GeneralPopUpManager.Initialize();
                coroutineOwner.StartCoroutine(ChangePwdCoroutine(data.email, oldPwd, newPwd));
            });
        };
        mainView.Add(changePassword);
        
        var deleteAccount = new Button { text = "Konto löschen" };
        deleteAccount.AddToClassList("button");
        deleteAccount.AddToClassList("settings-container");
        deleteAccount.clicked += () =>
        {
            GeneralPopUpManager.ShowDeleteAccount( (password) =>
            {
                GeneralPopUpManager.ResetInstance();
                GeneralPopUpManager.Initialize();
                coroutineOwner.StartCoroutine(DeleteAccount(data.email, password));
            });
        };
        mainView.Add(deleteAccount);

        var logoutBtn = new Button { text = "Abmelden" };
        logoutBtn.AddToClassList("button");
        logoutBtn.AddToClassList("settings-container");
        logoutBtn.clicked += () =>
        {
            GeneralPopUpManager.ShowConfirm("Bist du dir sicher?", "", () =>
            {
                GeneralUserDataManager.DeleteData();
                SceneManager.LoadScene("Authentication");
            });
        };

        mainView.Add(logoutBtn);
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator ChangePwdCoroutine(string email, string oldPwd, string newPwd)
    {
        var data = new GeneralSerializables.ChangePwdRequest(email, oldPwd, newPwd);
        GeneralLoadingSpinner.Show();
        var url = $"{PlayerPrefs.GetString("url")}/changePassword";
        var jsonData = JsonUtility.ToJson(data);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        GeneralLoadingSpinner.Hide();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Server wurde leider nicht erreicht!");
            yield break;
        }

        var response = JsonUtility.FromJson<GeneralSerializables.Response>(request.downloadHandler.text);

        if (response.success)
        {
            GeneralPopUpManager.ShowInfo("Geschafft!", "Dein Passwort wurde jetzt geändert");
            Debug.Log(response.message);
            GeneralUserDataManager.DeleteData();
            GeneralUserDataManager.SaveData(data.email, newPwd);
            
        }
        else
        {
            Debug.Log(response.message);
            GeneralPopUpManager.ShowInfo("Fehler!", response.error ?? "Passwort konnte nicht geändert werden!");
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator DeleteAccount(string email, string password)
    {
        var data = new GeneralSerializables.DeleteAccountRequest(email, password);
        GeneralLoadingSpinner.Show();
        var url = $"{PlayerPrefs.GetString("url")}/deleteAccount";
        var jsonData = JsonUtility.ToJson(data);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        GeneralLoadingSpinner.Hide();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Server wurde leider nicht erreicht!");
            yield break;
        }

        var response = JsonUtility.FromJson<GeneralSerializables.Response>(request.downloadHandler.text);

        if (response.success)
        {
            GeneralUserDataManager.DeleteData();
            SceneManager.LoadScene("Authentication");
        }
        else
        {
            Debug.Log(response.message);
            GeneralPopUpManager.ShowInfo("Fehler!", response.error ?? "Es gab einen Fehler beim löschen deines Kontos!");
        }
    }
}