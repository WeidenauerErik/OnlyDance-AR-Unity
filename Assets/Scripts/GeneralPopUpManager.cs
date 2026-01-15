using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralPopUpManager : MonoBehaviour
{
    private static GeneralPopUpManager _instance;

    private VisualElement _popupRoot;
    private Label _titleLabel;
    private VisualElement _popupInnerContainer;
    private Button _okButton;
    private Button _cancelButton;

    private Action _onYesCallback;
    private Action _onNoCallback;

    private static VisualElement _uiRoot;
    
    public static void Initialize()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }

        var uiDoc = FindFirstObjectByType<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("Kein UIDocument in der Szene gefunden!");
            return;
        }

        _uiRoot = uiDoc.rootVisualElement;
        var go = new GameObject("PopupManager");
        _instance = go.AddComponent<GeneralPopUpManager>();
        _instance.Setup();
    }

    private void Setup()
    {
        var styleSheet = Resources.Load<StyleSheet>("PopUp");
        if (styleSheet == null)
            Debug.LogError("PopUp.uss wurde nicht im Resources-Ordner gefunden!");
        var InputStyleSheet = Resources.Load<StyleSheet>("Input");
        if (InputStyleSheet == null)
            Debug.LogError("Input.uss wurde nicht im Resources-Ordner gefunden!");
        
        _popupRoot = new VisualElement { name = "popup-root" };
        _popupRoot.AddToClassList("popup-root");
        _popupRoot.style.display = DisplayStyle.None;

        var container = new VisualElement { name = "popup-container" };
        container.AddToClassList("popup-container");
        _popupRoot.Add(container);

        _titleLabel = new Label();
        _titleLabel.AddToClassList("text-large");
        container.Add(_titleLabel);

        _popupInnerContainer = new VisualElement();
        _popupInnerContainer.AddToClassList("popup-inner-container");
        container.Add(_popupInnerContainer);

        var buttonContainer = new VisualElement();
        buttonContainer.AddToClassList("button-select");
        container.Add(buttonContainer);
        
        _okButton = new Button();
        _okButton.AddToClassList("button");
        buttonContainer.Add(_okButton);

        _cancelButton = new Button();
        _cancelButton.AddToClassList("button");
        _cancelButton.style.display = DisplayStyle.None;
        buttonContainer.Add(_cancelButton);

        if (styleSheet != null)
        {
            _popupRoot.styleSheets.Add(styleSheet);
            _popupRoot.styleSheets.Add(InputStyleSheet);
        }

        _uiRoot.Add(_popupRoot);
    }

    public static void ResetInstance()
    {
        if (_instance == null) return;
        Destroy(_instance.gameObject);
        _instance = null;
    }
    
    public static void ShowInfo(string title, string message)
    {
        if (!_instance)
        {
            Debug.LogError("GeneralPopUpManager ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowInfo(title, message);
    }

    public static void ShowConfirm(string title, string message, Action onYes, Action onNo = null)
    {
        if (_instance == null)
        {
            Debug.LogError("GeneralPopUpManager ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowConfirm(title, message, onYes, onNo);
    }

    public static void ShowChangePassword(Action<string, string, string> onSubmit)
    {
        if (_instance == null)
        {
            Debug.LogError("GeneralPopUpManager ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowChangePassword(onSubmit);
    }

    public static void ShowDeleteAccount(Action<string> onSubmit)
    {
        if (_instance == null)
        {
            Debug.LogError("GeneralPopUpManager ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowDeleteAccount(onSubmit);
    }
	
	public static void ShowJsonImport(Action<string> onSubmit)
    {
        if (_instance == null)
        {
            Debug.LogError("GeneralPopUpManager ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowJsonImport(onSubmit);
    }

    public static void ShowDanceSettings(int danceID, VisualElement mainView)
    {
        if (_instance == null)
        {
            Debug.LogError("GeneralPopUpManager ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowDanceSettings(danceID, mainView);
    }

    private void InternalShowDeleteAccount(Action<string> onSubmit)
    {
        ClearCallbacks();
        _titleLabel.text = "Konto löschen";
        _popupInnerContainer.Clear();

        var password = CreatePasswordField("Passwort");
        var errorLabel = new Label();
        errorLabel.AddToClassList("error-label");

        _popupInnerContainer.Add(password);
        _popupInnerContainer.Add(errorLabel);

        _okButton.style.display = DisplayStyle.Flex;
        _okButton.text = "Löschen";
        _okButton.SetEnabled(false);
        _okButton.clicked -= HidePopup;
        _okButton.clicked += () =>
        {
            onSubmit?.Invoke(password.value);
            HidePopup();
        };

        _cancelButton.text = "Abbrechen";
        _cancelButton.style.display = DisplayStyle.Flex;
        _cancelButton.clicked -= HidePopup;
        _cancelButton.clicked += HidePopup;

        void Validate()
        {
            if (string.IsNullOrWhiteSpace(password.value))
            {
                errorLabel.text = "Bitte fülle das Feld aus!";
                _okButton.SetEnabled(false);
            }
            else
            {
                errorLabel.text = "";
                _okButton.SetEnabled(true);
            }
        }

        password.RegisterValueChangedCallback(_ => Validate());

        _popupRoot.style.display = DisplayStyle.Flex;
    }

    private void InternalShowInfo(string title, string message)
    {
        ClearCallbacks();
        _titleLabel.text = title;
        _popupInnerContainer.Clear();

        var messageLabel = new Label(message);
        messageLabel.AddToClassList("text-medium");
        
        _popupInnerContainer.Add(messageLabel);
        _okButton.style.display = DisplayStyle.Flex;
        _okButton.text = "OK";
        _okButton.clicked -= HidePopup;
        _okButton.clicked += HidePopup;

        _cancelButton.style.display = DisplayStyle.None;
        _popupRoot.style.display = DisplayStyle.Flex;
    }

    private void InternalShowConfirm(string title, string message, Action onYes, Action onNo)
    {
        ClearCallbacks();
        _titleLabel.text = title;
        _popupInnerContainer.Clear();
        
        var messageLabel = new Label(message);
        messageLabel.AddToClassList("text-medium");
        
        _popupInnerContainer.Add(messageLabel);

        _onYesCallback = onYes;
        _onNoCallback = onNo;

        _okButton.text = "Ja";
        _okButton.clicked -= OnYesPressed;
        _okButton.clicked += OnYesPressed;
        _okButton.SetEnabled(true);


        _cancelButton.text = "Nein";
        
        _cancelButton.style.display = DisplayStyle.Flex;
        _cancelButton.clicked -= OnNoPressed;
        _cancelButton.clicked += OnNoPressed;

        _popupRoot.style.display = DisplayStyle.Flex;
    }

    private void InternalShowChangePassword(Action<string, string, string> onSubmit)
    {
        ClearCallbacks();
        _titleLabel.text = "Passwort ändern";
        _popupInnerContainer.Clear();

        var oldPw = CreatePasswordField("Altes Passwort");
        var newPw = CreatePasswordField("Neues Passwort");
        var confirmPw = CreatePasswordField("Passwort wiederholen");
        var errorLabel = new Label();
        errorLabel.AddToClassList("error-label");

        _popupInnerContainer.Add(oldPw);
        _popupInnerContainer.Add(newPw);
        _popupInnerContainer.Add(confirmPw);
        _popupInnerContainer.Add(errorLabel);

        _okButton.style.display = DisplayStyle.Flex;
        _okButton.text = "Ändern";
        _okButton.SetEnabled(false);
        _okButton.clicked -= HidePopup;
        _okButton.clicked += () =>
        {
            onSubmit?.Invoke(oldPw.value, newPw.value, confirmPw.value);
            HidePopup();
        };

        _cancelButton.text = "Abbrechen";
        _cancelButton.style.display = DisplayStyle.Flex;
        _cancelButton.clicked -= HidePopup;
        _cancelButton.clicked += HidePopup;

        void Validate()
        {
            if (string.IsNullOrWhiteSpace(oldPw.value) ||
                string.IsNullOrWhiteSpace(newPw.value) ||
                string.IsNullOrWhiteSpace(confirmPw.value))
            {
                errorLabel.text = "Bitte alle Felder ausfüllen!";
                _okButton.SetEnabled(false);
            }
            else if (newPw.value.Length < 6)
            {
                errorLabel.text = "Passwort muss mindestens 6 Zeichen lang sein!";
                _okButton.SetEnabled(false);
            }
            else if (newPw.value != confirmPw.value)
            {
                errorLabel.text = "Passwörter stimmen nicht überein!";
                _okButton.SetEnabled(false);
            }
            else
            {
                errorLabel.text = "";
                _okButton.SetEnabled(true);
            }
        }

        oldPw.RegisterValueChangedCallback(_ => Validate());
        newPw.RegisterValueChangedCallback(_ => Validate());
        confirmPw.RegisterValueChangedCallback(_ => Validate());

        _popupRoot.style.display = DisplayStyle.Flex;
    }

    private void InternalShowJsonImport(Action<string> onSubmit)
{
    ClearCallbacks();
    _titleLabel.text = "Import";
    _popupInnerContainer.Clear();

    var jsonField = new TextField { multiline = true };
    jsonField.AddToClassList("textfield");
    jsonField.textEdition.placeholder = "Füge deinen JSON hier ein...";

    var errorLabel = new Label();
    errorLabel.AddToClassList("error-label");

    _popupInnerContainer.Add(jsonField);
    _popupInnerContainer.Add(errorLabel);

    _okButton.style.display = DisplayStyle.Flex;
    _okButton.text = "Importieren";
    _okButton.SetEnabled(false);
    
    _okButton.clicked -= OnImportClicked;
    _okButton.clicked += OnImportClicked;
    
    void OnImportClicked()
    {
        var tempDance = JsonUtility.FromJson<GeneralSerializables.DanceData>(jsonField.value);
        GeneralDanceDataManager.SaveDance(tempDance);
        onSubmit?.Invoke(jsonField.value);
        HidePopup();
    }
    
    _cancelButton.text = "Abbrechen";
    _cancelButton.style.display = DisplayStyle.Flex;
    _cancelButton.clicked -= HidePopup;
    _cancelButton.clicked += HidePopup;

    bool Validate(string json, out string errorMessage)
{
    errorMessage = "";

    if (string.IsNullOrWhiteSpace(json))
    {
        errorMessage = "JSON ist leer!";
        return false;
    }

    GeneralSerializables.DanceData dance;

    try
    {
        dance = JsonUtility.FromJson<GeneralSerializables.DanceData>(json);
    }
    catch (Exception e)
    {
        errorMessage = "JSON Fehler: " + e.Message;
        return false;
    }

    if (dance == null)
    {
        errorMessage = "JSON konnte nicht geparst werden!";
        return false;
    }

    if (string.IsNullOrWhiteSpace(dance.name))
    {
        errorMessage = "Fehler: 'name' fehlt oder ist leer.";
        return false;
    }

    if (dance.BPM <= 0)
    {
        errorMessage = "Fehler: 'BPM' muss größer als 0 sein.";
        return false;
    }

    if (dance.data == null || dance.data.Count == 0)
    {
        errorMessage = "Fehler: Die 'data' Liste ist leer.";
        return false;
    }

    for (int i = 0; i < dance.data.Count; i++)
    {
        var step = dance.data[i];

        if (step == null)
        {
            errorMessage = $"Fehler in Schritt {i + 1}: Schritt ist null!";
            return false;
        }

        if (step.id <= 0)
        {
            errorMessage = $"Fehler in Schritt {i + 1}: 'id' ungültig.";
            return false;
        }

        if (float.IsNaN(step.m1_x) || float.IsNaN(step.m1_y) || float.IsNaN(step.m1_rotate) ||
            float.IsNaN(step.m2_x) || float.IsNaN(step.m2_y) || float.IsNaN(step.m2_rotate))
        {
            errorMessage = $"Fehler in Schritt {i + 1}: Ungültige Werte.";
            return false;
        }

        if (step.m1_toe && step.m1_heel)
        {
            errorMessage = $"Fehler in Schritt {i + 1}: m1 kann nicht toe UND heel sein.";
            return false;
        }

        if (step.m2_toe && step.m2_heel)
        {
            errorMessage = $"Fehler in Schritt {i + 1}: m2 kann nicht toe UND heel sein.";
            return false;
        }
    }

    return true;
}

    jsonField.RegisterValueChangedCallback(evt => {
    if (!Validate(evt.newValue, out string error))
    {
        errorLabel.text = error;
        _okButton.SetEnabled(false);
    }
    else
    {
        errorLabel.text = "";
        _okButton.SetEnabled(true);
    }
});

    _popupRoot.style.display = DisplayStyle.Flex;
}

    private void InternalShowDanceSettings(int danceID, VisualElement mainView)
    {
        ClearCallbacks();
        _titleLabel.text = "Tanz-Einstellungen";
        _popupInnerContainer.Clear();
        
        var messageLabel = new Label("Hier kannst du die Einstellungen für deinen Tanz ändern.");
        messageLabel.AddToClassList("text-medium-grey-2");
        _popupInnerContainer.Add(messageLabel);
        
        _okButton.style.display = DisplayStyle.None;
        
        _cancelButton.text = "Abbrechen";
        _cancelButton.style.display = DisplayStyle.Flex;
        _cancelButton.clicked -= HidePopup;
        _cancelButton.clicked += HidePopup;
        
        var buttonContainer = new VisualElement();
        buttonContainer.AddToClassList("popup-container");
        _popupInnerContainer.Add(buttonContainer);
        
        var exportBtn = new Button();
        exportBtn.text = "Exportieren";
        exportBtn.AddToClassList("button");
        exportBtn.clicked += () =>
        {
            Debug.Log("Exportieren");
            HidePopup();
        };
        buttonContainer.Add(exportBtn);
        
        var editBtn = new Button();
        editBtn.text = "Bearbeiten";
        editBtn.AddToClassList("button");
        editBtn.clicked += () =>
        {
            Debug.Log("Bearbeiten");
            HidePopup();
        };
        buttonContainer.Add(editBtn);
        
        var deleteBtn = new Button();
        deleteBtn.text = "Löschen";
        deleteBtn.AddToClassList("button");
        deleteBtn.clicked += () =>
        {
            GeneralDanceDataManager.DeleteDance(danceID);
            HidePopup();
            MainMenuDanceManager.SetMyDancesIntoView(mainView);
        };
        buttonContainer.Add(deleteBtn);

        _popupRoot.style.display = DisplayStyle.Flex;
    }
    
    private static TextField CreatePasswordField(string label)
    {
        var field = new TextField { isPasswordField = true };
        field.textEdition.placeholder = label;
        field.AddToClassList("input");
        return field;
    }

    private void OnYesPressed()
    {
        _onYesCallback?.Invoke();
        HidePopup();
    }

    private void OnNoPressed()
    {
        _onNoCallback?.Invoke();
        HidePopup();
    }

    private void HidePopup()
    {
        _popupRoot.style.display = DisplayStyle.None;
        ClearCallbacks();
    }

    private void ClearCallbacks()
    {
        _okButton.clicked -= OnYesPressed;
        _okButton.clicked -= HidePopup;
        _cancelButton.clicked -= OnNoPressed;
        _cancelButton.clicked -= HidePopup;
        _onYesCallback = null;
        _onNoCallback = null;
    }
}