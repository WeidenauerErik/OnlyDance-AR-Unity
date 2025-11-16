using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private Button _myDancesBtn;
    private Label _myDancesBtnLabel;
    private VisualElement _myDancesBtnIcon;

    private Button _onlineDancesBtn;
    private Label _onlineDancesBtnLabel;
    private VisualElement _onlineDancesBtnIcon;

    private Button _settingsBtn;
    private Label _settingsBtnLabel;
    private VisualElement _settingsBtnIcon;

    private VisualElement _mainContent;
    
    void Start()
    {
        var uiDoc = FindFirstObjectByType<UIDocument>();
        var root = uiDoc.rootVisualElement;

        GeneralLoadingSpinner.Initialize(root);
        GeneralPopUpManager.Initialize();
     
        _mainContent = root.Q<VisualElement>("mainContent");

        _myDancesBtn = root.Q<Button>("myDancesBtn");
        _myDancesBtn.clicked += MyDancesBtnClicked;
        _myDancesBtnLabel = root.Q<Label>("myDancesBtnLabel");
        _myDancesBtnIcon = root.Q<VisualElement>("myDancesBtnIcon");

        _onlineDancesBtn = root.Q<Button>("onlineDancesBtn");
        _onlineDancesBtn.clicked += OnlineDancesBtnClicked;
        _onlineDancesBtnLabel = root.Q<Label>("onlineDancesBtnLabel");
        _onlineDancesBtnIcon = root.Q<VisualElement>("onlineDancesBtnIcon");

        _settingsBtn = root.Q<Button>("settingsBtn");
        _settingsBtn.clicked += SettingsBtnClicked;
        _settingsBtnLabel = root.Q<Label>("settingsBtnLabel");
        _settingsBtnIcon = root.Q<VisualElement>("settingsBtnIcon");

        MyDancesBtnClicked();
    }

    private void MyDancesBtnClicked()
    {
        MainMenuDanceManager.SetMyDancesIntoView(_mainContent);

        _myDancesBtnLabel.AddToClassList("activeLabel");
        _onlineDancesBtnLabel.RemoveFromClassList("activeLabel");
        _settingsBtnLabel.RemoveFromClassList("activeLabel");

        _myDancesBtnIcon.AddToClassList("activeMyDances");
        _onlineDancesBtnIcon.RemoveFromClassList("activeOnlineDances");
        _settingsBtnIcon.RemoveFromClassList("activeSettings");
    }
    
    private void OnlineDancesBtnClicked()
    {
        MainMenuDanceManager.SetOnlineDancesIntoView(_mainContent);

        _myDancesBtnLabel.RemoveFromClassList("activeLabel");
        _onlineDancesBtnLabel.AddToClassList("activeLabel");
        _settingsBtnLabel.RemoveFromClassList("activeLabel");

        _myDancesBtnIcon.RemoveFromClassList("activeMyDances");
        _onlineDancesBtnIcon.AddToClassList("activeOnlineDances");
        _settingsBtnIcon.RemoveFromClassList("activeSettings");
    }

    private void SettingsBtnClicked()
    {
        MainMenuSettingsManager.SetSettingsIntoView(_mainContent, this);

        _myDancesBtnLabel.RemoveFromClassList("activeLabel");
        _onlineDancesBtnLabel.RemoveFromClassList("activeLabel");
        _settingsBtnLabel.AddToClassList("activeLabel");

        _myDancesBtnIcon.RemoveFromClassList("activeMyDances");
        _onlineDancesBtnIcon.RemoveFromClassList("activeOnlineDances");
        _settingsBtnIcon.AddToClassList("activeSettings");
    }

    public static Label CreateHeading(string labelName)
    {
        var tmp = new Label(labelName);
        tmp.AddToClassList("text-large");
        return tmp;
    }
}