using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Serializable]
public class ChangePwdRequest
{
    public string email;
    public string oldPassword;
    public string newPassword;

    public ChangePwdRequest(string email, string oldPassword, string newPassword)
    {
        this.email = email;
        this.oldPassword = oldPassword;
        this.newPassword = newPassword;
    }
}

[Serializable]
public class DeleteAccountRequest
{
    public string email;
    public string password;

    public DeleteAccountRequest(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
}

public class SettingsManagerMainMenu : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView, MonoBehaviour coroutineOwner)
    {
        mainView.Clear();
        mainView.Add(MainMenu.CreateHeading("Settings"));

        var data = DataManagerGeneral.LoadData();
        var emailLabel = new Label { text = data.email };
        emailLabel.AddToClassList("settings-emailLabel");

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
            PopUpManagerGeneral.ShowChangePassword((oldPwd, newPwd, confirmPwd) =>
            {
                PopUpManagerGeneral.ResetInstance();
                PopUpManagerGeneral.Initialize();
                coroutineOwner.StartCoroutine(ChangePwdCoroutine(data.email, oldPwd, newPwd));
            });
        };
        mainView.Add(changePassword);
        
        var deleteAccount = new Button { text = "Konto löschen" };
        deleteAccount.AddToClassList("button");
        deleteAccount.AddToClassList("settings-container");
        deleteAccount.clicked += () =>
        {
            PopUpManagerGeneral.ShowDeleteAccount( (password) =>
            {
                PopUpManagerGeneral.ResetInstance();
                PopUpManagerGeneral.Initialize();
                coroutineOwner.StartCoroutine(DeleteAccount(data.email, password));
            });
        };
        mainView.Add(deleteAccount);

        var logoutBtn = new Button { text = "Logout" };
        logoutBtn.AddToClassList("button");
        logoutBtn.AddToClassList("settings-container");
        logoutBtn.clicked += () =>
        {
            PopUpManagerGeneral.ShowConfirm("Bist du dir sicher?", "", () =>
            {
                Debug.Log("Logged out");
                DataManagerGeneral.DeleteData();
                SceneManager.LoadScene("Authentication");
            });
        };

        mainView.Add(logoutBtn);
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator ChangePwdCoroutine(string email, string oldPwd, string newPwd)
    {
        var data = new ChangePwdRequest(email, oldPwd, newPwd);
        LoadingSpinnerGeneral.Show();
        var url = $"{PlayerPrefs.GetString("url")}/changePassword";
        var jsonData = JsonUtility.ToJson(data);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        LoadingSpinnerGeneral.Hide();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Server wurde leider nicht erreicht!");
            yield break;
        }

        var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);

        if (response.success)
        {
            PopUpManagerGeneral.ShowInfo("Geschafft!", "Dein Passwort wurde jetzt geändert");
            Debug.Log(response.message);
            DataManagerGeneral.DeleteData();
            DataManagerGeneral.SaveData(data.email, newPwd);
            
        }
        else
        {
            Debug.Log(response.message);
            PopUpManagerGeneral.ShowInfo("Fehler!", response.error ?? "Passwort konnte nicht geändert werden!");
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator DeleteAccount(string email, string password)
    {
        var data = new DeleteAccountRequest(email, password);
        LoadingSpinnerGeneral.Show();
        var url = $"{PlayerPrefs.GetString("url")}/deleteAccount";
        var jsonData = JsonUtility.ToJson(data);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        LoadingSpinnerGeneral.Hide();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            PopUpManagerGeneral.ShowInfo("Fehler!", "Server wurde leider nicht erreicht!");
            yield break;
        }

        var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);

        if (response.success)
        {
            DataManagerGeneral.DeleteData();
            SceneManager.LoadScene("Authentication");
        }
        else
        {
            Debug.Log(response.message);
            PopUpManagerGeneral.ShowInfo("Fehler!", response.error ?? "Es gab einen Fehler beim löschen deines Kontos!");
        }
    }
}