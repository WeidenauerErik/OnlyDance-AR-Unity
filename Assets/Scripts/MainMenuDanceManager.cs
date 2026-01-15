using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuDanceManager : MonoBehaviour
{
    private static int _selectedDanceSchoolId;
    
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
        try
        {
            GeneralLoadingSpinner.Show();
            mainView.Clear();

            var danceSchools = await FetchDanceSchoolsByEmail(GeneralUserDataManager.LoadData().email);

            if (danceSchools == null || danceSchools.Count == 0)
            {
                var label = new Label("Du bist keiner Tanzschule zugeordnet.");
                label.AddToClassList("text-medium-grey-2");
                mainView.Add(label);

                GeneralLoadingSpinner.Hide();
                return;
            }
            
            if (ChangeDanceSchoolDataTransfer.Instance != null &&
                ChangeDanceSchoolDataTransfer.Instance.SelectedDanceSchoolId != 0)
            {
                _selectedDanceSchoolId = ChangeDanceSchoolDataTransfer.Instance.SelectedDanceSchoolId;
            }
            else
            {
                _selectedDanceSchoolId = danceSchools[0].id;
            }

            await RenderSelectedSchool(mainView, danceSchools);

            GeneralLoadingSpinner.Hide();
        }
        catch
        {
            GeneralLoadingSpinner.Hide();
            GeneralPopUpManager.ShowInfo("Fehler!", "Es konnte leider keine Internetverbindung hergestellt werden.");
        }
    }
    
    private static async Task RenderSelectedSchool(VisualElement mainView, List<GeneralSerializables.Dance> danceSchools)
    {
        try
        {
            GeneralLoadingSpinner.Show();
            mainView.Clear();
            
            var selected = danceSchools.Find(ds => ds.id == _selectedDanceSchoolId);
            if (selected == null)
            {
                _selectedDanceSchoolId = danceSchools[0].id;
                selected = danceSchools[0];
            }

            switch (danceSchools.Count)
            {
                case <= 1:
                {
                    var onlineDancesLabel = new Label("Online Tänze");
                    onlineDancesLabel.AddToClassList("text-large");
                    mainView.Add(onlineDancesLabel);
                    break;
                }
                case > 1:
                {
                    var switchBtnContainer = new VisualElement();
                    switchBtnContainer.AddToClassList("heading-navbar");

                    var selectedDanceSchool = new Label(selected.name);
                    selectedDanceSchool.AddToClassList("text-large");

                    switchBtnContainer.Add(selectedDanceSchool);
                    
                    var switchIcon = new Button();
                    switchIcon.AddToClassList("switchIcon");
                    switchIcon.RemoveFromClassList("unity-button");
                    switchIcon.clicked += () =>
                    {
                        if (ChangeDanceSchoolDataTransfer.Instance == null)
                        {
                            Debug.LogError("ChangeDanceSchoolDataTransfer.Instance ist NULL! Bitte DataTransfer Objekt in Startscene hinzufügen.");
                            return;
                        }

                        ChangeDanceSchoolDataTransfer.Instance.SetDanceSchools(danceSchools, _selectedDanceSchoolId);
                        SceneManager.LoadScene("ChangeDanceSchool");
                    };
                    mainView.Add(switchBtnContainer);
                    switchBtnContainer.Add(switchIcon);
                    break;
                }
            }
            
            var url = $"{PlayerPrefs.GetString("url")}/getAllDances/{_selectedDanceSchoolId}";
            var dances = await FetchDances(url);

            if (dances == null || dances.Count == 0)
            {
                var error = new Label("Es wurden leider noch keine Tänze für diese Tanzschule erstellt.");
                error.AddToClassList("text-medium-grey-2");
                mainView.Add(error);
                return;
            }
            
            CreateDance(mainView, dances, true);
        }
        catch
        {
            GeneralPopUpManager.ShowInfo("Fehler!", "Die Tänze konnten nicht geladen werden.");
        }
        finally
        {
            GeneralLoadingSpinner.Hide();
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
                MainMenuDanceDataTransfer.Instance.SetDanceCredentials(dance.name, dance.id, isOnlineDance);
                SceneManager.LoadScene("DanceAnimator");
            };
            
            if (!isOnlineDance)
            {
                PlayerPrefs.SetString("locationMainMenu", "myDances");
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
            }
            else
            {
                PlayerPrefs.SetString("locationMainMenu", "onlineDances");
                container.Add(dancePlayBtn);
            }

            danceContainer.Add(container);
        }

        mainView.Add(danceContainer);
    }
}
