using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuDanceManager : MonoBehaviour
{
    private static int? _selectedDanceSchoolId;

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
            GeneralPopUpManager.ShowJsonImport(json => { SetMyDancesIntoView(mainView); });
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

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                GeneralLoadingSpinner.Hide();
                GeneralPopUpManager.ShowInfo("Fehler!",
                    "Es konnte leider keine Internetverbindung hergestellt werden.");
                return;
            }

            var danceSchools = await FetchDanceSchoolsByEmail(GeneralUserDataManager.LoadData().email);

            if (danceSchools.Count > 0)
                _selectedDanceSchoolId = danceSchools[0].id;

            SetDancesForSelectedSchool(mainView, danceSchools);

            GeneralLoadingSpinner.Hide();
        }
        catch
        {
            GeneralLoadingSpinner.Hide();
            GeneralPopUpManager.ShowInfo("Fehler!", "Die DanceSchools konnten nicht geladen werden.");
        }
    }

    private static async void SetDancesForSelectedSchool(VisualElement mainView,
        List<GeneralSerializables.Dance> danceSchools)
    {
        if (_selectedDanceSchoolId == null)
            return;

        GeneralLoadingSpinner.Show();

        try
        {
            var url = $"{PlayerPrefs.GetString("url")}/getAllDances/{_selectedDanceSchoolId}";
            var dances = await FetchDances(url);

            mainView.Clear();

            if (danceSchools.Count > 1)
            {
                var dropdown = new DropdownField();
                dropdown.choices = danceSchools.ConvertAll(ds => ds.name);
                mainView.Add(dropdown);
                var selectedSchool = danceSchools.Find(ds => ds.id == _selectedDanceSchoolId.Value);
                if (selectedSchool != null)
                    dropdown.SetValueWithoutNotify(selectedSchool.name);
                dropdown.RegisterValueChangedCallback(evt =>
                {
                    var selected = danceSchools.Find(ds => ds.name == evt.newValue);
                    if (selected != null)
                        _selectedDanceSchoolId = selected.id;

                    SetDancesForSelectedSchool(mainView, danceSchools);
                });
                if (dances.Count == 0)
                {
                    var error = new Label("Es wurden leider noch keine Tänze für diese Tanzschule erstellt.");
                    error.AddToClassList("text-medium-grey-2");
                    mainView.Add(error);
                }
                else CreateDance(mainView, dances, true);
            }
            else
            {
                mainView.Add(MainMenu.CreateHeading("Online Tänze"));
                if (dances.Count == 0)
                {
                    var error = new Label("Es wurden leider noch keine Tänze für diese Tanzschule erstellt.");
                    error.AddToClassList("text-medium-grey-2");
                    mainView.Add(error);
                }
                else CreateDance(mainView, dances, true);
            }
            GeneralLoadingSpinner.Hide();
        }
        catch

        {
            GeneralLoadingSpinner.Hide();
            GeneralPopUpManager.ShowInfo("Fehler!", "Die Tänze konnten nicht geladen werden.");
        }
    }

    private static async Task<List<GeneralSerializables.Dance>> FetchDanceSchoolsByEmail(string email)
    {
        var url = $"{PlayerPrefs.GetString("url")}/getUserDanceSchoolsByEmail/{email}";
        using var request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception(request.error);

        var json = request.downloadHandler.text;
        var wrapper = JsonUtility.FromJson<GeneralSerializables.DanceWrapper>(json);
        return new List<GeneralSerializables.Dance>(wrapper.data);
    }

    private static async Task<List<GeneralSerializables.Dance>> FetchDances(string url)
    {
        using var request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception(request.error);

        var json = request.downloadHandler.text;
        var wrapper = JsonUtility.FromJson<GeneralSerializables.DanceWrapper>(json);

        return new List<GeneralSerializables.Dance>(wrapper.data);
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
                settingsBtn.clicked += () => { GeneralPopUpManager.ShowDanceSettings(dance.id, mainView); };

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