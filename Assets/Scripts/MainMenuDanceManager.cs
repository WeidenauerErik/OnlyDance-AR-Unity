using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuDanceManager : MonoBehaviour
{
    public static void SetMyDancesIntoView(VisualElement mainView)
    {
        var myDanceList = GeneralDanceDataManager.GetAllDances();
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
                GeneralPopUpManager.ResetInstance();
                GeneralPopUpManager.Initialize();
                GeneralPopUpManager.ShowJsonImport(json =>
                {
                    SetMyDancesIntoView(mainView);
                });
        };
        headingContainer.Add(importButton);
        
        mainView.Add(headingContainer);

        if (myDanceList.Count == 0)
        {
            var error = new Label("Du musst als erstes Tänze erstellen, sodass du eigene tanzen kannst.");
            error.AddToClassList("text-medium-grey-2");
            mainView.Add(error);
        }
        else
        {
            CreateDance(mainView, myDanceList, false);
        }
    }

    public static async void SetOnlineDancesIntoView(VisualElement mainView)
    {
        GeneralLoadingSpinner.Show();
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
                if (dances.Count == 0)
                {
                    var error = new Label("Es wurden leider noch keine Tänze erstellt.");
                    error.AddToClassList("text-medium-grey-2");
                    mainView.Add(error);
                }
                else CreateDance(mainView, dances, true);
                GeneralLoadingSpinner.Hide();
            }
            catch (Exception e)
            {
              Debug.LogError($"Fehler beim Laden der Tänze: {e.Message}");
              GeneralLoadingSpinner.Hide();
              GeneralPopUpManager.ShowInfo("Fehler!", "Die Online Tänze konnten nicht geladen werden.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            GeneralLoadingSpinner.Hide();
            GeneralPopUpManager.ShowInfo("Fehler!", "Die Online Tänze konnten nicht geladen werden.");
        }
    }

    private static async Task<List<GeneralSerializables.Dance>> FetchFiveDances(string url)
    {
        using var request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success) throw new Exception(request.error);

        var json = request.downloadHandler.text;
        var wrappedJson = "{\"dances\":" + json + "}";
        var wrapper = JsonUtility.FromJson<GeneralSerializables.DanceWrapper>(wrappedJson);

        return new List<GeneralSerializables.Dance>(wrapper.dances);
    }

    private static void CreateDance(VisualElement mainView, IEnumerable<GeneralSerializables.Dance> danceList, bool isOnlineDance)
    {
		var danceContainer = new VisualElement();
		danceContainer.AddToClassList("dance-container");
		
        foreach (var dance in danceList)
        {
            var container = new VisualElement();
            container.AddToClassList("dance");

            var danceNameLabel = new Label(dance.name);
            danceNameLabel.AddToClassList("danceName");
            container.Add(danceNameLabel);
			
            var dancePlayBtn = new Button();
            dancePlayBtn.AddToClassList("dancePlayButton");
            dancePlayBtn.RemoveFromClassList("unity-button");
            dancePlayBtn.clicked += () =>
            {
                MainMenuDanceLoader.Instance.SetDanceCredentials(dance.name, dance.id, isOnlineDance);
                SceneManager.LoadScene("DanceAnimator");
            };
            if (!isOnlineDance) 
            {
                var btnContainer = new VisualElement();
                btnContainer.AddToClassList("dance-button-container");
                
                var settingsBtn = new Button();
                settingsBtn.AddToClassList("danceSettingsButton");
                settingsBtn.RemoveFromClassList("unity-button");
                settingsBtn.clicked += () =>
                {
                    GeneralPopUpManager.ShowDanceSettings(dance.id, mainView);
                };

                btnContainer.Add(dancePlayBtn);
                btnContainer.Add(settingsBtn);

                container.Add(btnContainer);
                danceContainer.Add(container);
            }
            else
            {
                container.Add(dancePlayBtn);
                danceContainer.Add(container);
            }
        }
		mainView.Add(danceContainer);
    }
}