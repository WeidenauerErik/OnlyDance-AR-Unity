using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUpManagerGeneral : MonoBehaviour
{
    private static PopUpManagerGeneral _instance;

    private VisualElement _popupRoot;
    private Label _titleLabel;
    private VisualElement _messageContainer;
    private Button _okButton;
    private Button _cancelButton;

    private Action _onYesCallback;
    private Action _onNoCallback;

    private static VisualElement _uiRoot;

    [Obsolete("Obsolete")]
    public static void Initialize()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }

        var uiDoc = FindObjectOfType<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("Kein UIDocument in der Szene gefunden!");
            return;
        }

        _uiRoot = uiDoc.rootVisualElement;
        var go = new GameObject("PopupManager");
        _instance = go.AddComponent<PopUpManagerGeneral>();
        _instance.Setup();
    }

    private void Setup()
    {
        // Stylesheet laden
        var styleSheet = Resources.Load<StyleSheet>("PopUp");
        if (styleSheet == null)
            Debug.LogError("PopUp.uss wurde nicht im Resources-Ordner gefunden!");

        // Popup Root
        _popupRoot = new VisualElement { name = "popup-root" };
        _popupRoot.AddToClassList("popup-root");
        _popupRoot.style.display = DisplayStyle.None;

        var container = new VisualElement { name = "popup-container" };
        container.AddToClassList("popup-container");
        _popupRoot.Add(container);

        _titleLabel = new Label { name = "popup-title" };
        _titleLabel.AddToClassList("popup-title");
        container.Add(_titleLabel);

        _messageContainer = new Label { name = "popup-message" };
        _messageContainer.AddToClassList("popup-message");
        container.Add(_messageContainer);

        var buttonContainer = new VisualElement { name = "popup-button-container" };
        buttonContainer.AddToClassList("popup-button-container");
        container.Add(buttonContainer);

        _okButton = new Button { name = "popup-ok" };
        _okButton.AddToClassList("popup-button");
        buttonContainer.Add(_okButton);

        _cancelButton = new Button { name = "popup-cancel" };
        _cancelButton.AddToClassList("popup-button");
        _cancelButton.style.display = DisplayStyle.None;
        buttonContainer.Add(_cancelButton);

        if (styleSheet != null)
            _popupRoot.styleSheets.Add(styleSheet);

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
        if (_instance == null)
        {
            Debug.LogError("PopUpManagerGeneral ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowInfo(title, message);
    }

    public static void ShowConfirm(string title, string message, Action onYes, Action onNo = null)
    {
        if (_instance == null)
        {
            Debug.LogError("PopUpManagerGeneral ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowConfirm(title, message, onYes, onNo);
    }

    public static void ShowChangePassword(Action<string, string, string> onSubmit)
    {
        if (_instance == null)
        {
            Debug.LogError("PopUpManagerGeneral ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowChangePassword(onSubmit);
    }

    public static void ShowDeleteAccount(Action<string> onSubmit)
    {
        if (_instance == null)
        {
            Debug.LogError("PopUpManagerGeneral ist nicht initialisiert!");
            return;
        }
        _instance.InternalShowDeleteAccount(onSubmit);
    }

    private void InternalShowDeleteAccount(Action<string> onSubmit)
    {
        ClearCallbacks();
        _titleLabel.text = "Konto löschen";
        _messageContainer.Clear();

        var password = CreatePasswordField("Passwort");
        var errorLabel = new Label();
        errorLabel.AddToClassList("error-label");

        _messageContainer.Add(password);
        _messageContainer.Add(errorLabel);

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
        _messageContainer.Clear();
        _messageContainer.Add(new Label(message));
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
        _messageContainer.Clear();
        _messageContainer.Add(new Label(message));

        _onYesCallback = onYes;
        _onNoCallback = onNo;

        _okButton.text = "Ja";
        _okButton.clicked -= OnYesPressed;
        _okButton.clicked += OnYesPressed;

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
        _messageContainer.Clear();

        var oldPw = CreatePasswordField("Altes Passwort");
        var newPw = CreatePasswordField("Neues Passwort");
        var confirmPw = CreatePasswordField("N. Passwort bestätigen");
        var errorLabel = new Label();
        errorLabel.AddToClassList("error-label");

        _messageContainer.Add(oldPw);
        _messageContainer.Add(newPw);
        _messageContainer.Add(confirmPw);
        _messageContainer.Add(errorLabel);

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

    private static TextField CreatePasswordField(string label)
    {
        var field = new TextField(label) { isPasswordField = true };
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
