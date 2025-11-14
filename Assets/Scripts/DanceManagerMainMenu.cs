using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Serializable]
public class Dance
{
    public int id;
    public string name;
}

[Serializable]
public class DanceWrapper
{
    public Dance[] dances;
}

public class DanceManagerMainMenu : MonoBehaviour
{
    public static void SetMyDancesIntoView(VisualElement mainView)
    {
        var myDanceList = new List<Dance>
        {
            new Dance { id = 1, name = "Weidi Cha Cha" }
        };

        mainView.Clear();

        var headingContainer = new VisualElement();
        headingContainer.AddToClassList("heading-navbar");
        
        headingContainer.Add(new VisualElement());
        headingContainer.Add(MainMenu.CreateHeading("Meine Tänze"));
        
        var importButton = new Button();
        importButton.AddToClassList("importJsonButton");
        importButton.RemoveFromClassList("unity-button");
        importButton.clicked += () =>
        {
            PopUpManagerGeneral.ShowJsonImport(json =>
            {
                Debug.Log(json);
            });
        };
        headingContainer.Add(importButton);
        
        mainView.Add(headingContainer);
        CreateDance(mainView, myDanceList);
    }

    public static async void SetOnlineDancesIntoView(VisualElement mainView)
    {
        LoadingSpinnerGeneral.Show();
        try
        {
            mainView.Clear();
            mainView.Add(MainMenu.CreateHeading("Online Tänze"));

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                var tmpNetworkError = new Label("There is no internet connection!");
                tmpNetworkError.AddToClassList("networkError");
                mainView.Add(tmpNetworkError);
                return;
            }

            try
            {
                var url = $"{PlayerPrefs.GetString("url")}/getFiveDances";
                var dances = await FetchFiveDances(url);
                mainView.Clear();
                mainView.Add(MainMenu.CreateHeading("Online Tänze"));
                CreateDance(mainView, dances);
                LoadingSpinnerGeneral.Hide();
            }
            catch (Exception e)
            {
              Debug.LogError($"Fehler beim Laden der Tänze: {e.Message}");
              LoadingSpinnerGeneral.Hide();
              PopUpManagerGeneral.ShowInfo("Fehler!", "Die Online Tänze konnten nicht geladen werden.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            LoadingSpinnerGeneral.Hide();
            PopUpManagerGeneral.ShowInfo("Fehler!", "Die Online Tänze konnten nicht geladen werden.");
        }
    }

    private static async Task<List<Dance>> FetchFiveDances(string url)
    {
        using var request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success) throw new Exception(request.error);

        var json = request.downloadHandler.text;
        var wrappedJson = "{\"dances\":" + json + "}";
        var wrapper = JsonUtility.FromJson<DanceWrapper>(wrappedJson);

        return new List<Dance>(wrapper.dances);
    }

    private static void CreateDance(VisualElement mainView, IEnumerable<Dance> danceList)
    {
        foreach (var dance in danceList)
        {
            var container = new VisualElement();
            container.AddToClassList("danceContainer");

            var danceNameLabel = new Label(dance.name);
            danceNameLabel.AddToClassList("danceName");
            container.Add(danceNameLabel);

            var dancePlayBtn = new Button();
            dancePlayBtn.AddToClassList("dancePlayButton");
            dancePlayBtn.RemoveFromClassList("unity-button");
            dancePlayBtn.clicked += () =>
            {
                DanceLoaderMainMenu.Instance.SetDanceCredentials(dance.name, dance.id);
                SceneManager.LoadScene("DanceAnimator");
            };

            container.Add(dancePlayBtn);
            mainView.Add(container);
        }
    }
}