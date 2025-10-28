using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUpManagerGeneral : MonoBehaviour
{
    private VisualElement _popupRoot;
    private Label _titleLabel;
    private Label _messageLabel;
    private Button _okButton;
    private Button _cancelButton;

    private Action _onYesCallback;
    private Action _onNoCallback;

    private VisualElement _uiRoot;

    private static PopUpManagerGeneral _currentInstance;

    /// <summary>
    /// Erstellt und initialisiert in jeder Szene eine neue PopupManager-Instanz.
    /// </summary>
    public static void Initialize(VisualElement root)
    {
        // Falls bereits eine alte Instanz existiert, löschen wir sie sauber
        if (_currentInstance != null)
        {
            Destroy(_currentInstance.gameObject);
            _currentInstance = null;
        }

        var go = new GameObject("PopupManager");
        _currentInstance = go.AddComponent<PopUpManagerGeneral>();
        _currentInstance.Setup(root);
    }

    private void Setup(VisualElement root)
    {
        _uiRoot = root;

        var styleSheet = Resources.Load<StyleSheet>("PopUp");
        if (styleSheet == null)
        {
            Debug.LogError("PopUp.uss wurde nicht im Resources-Ordner gefunden!");
        }

        // Popup Grundstruktur
        _popupRoot = new VisualElement { name = "popup-root" };
        _popupRoot.AddToClassList("popup-root");

        var container = new VisualElement { name = "popup-container" };
        container.AddToClassList("popup-container");
        _popupRoot.Add(container);

        _titleLabel = new Label("Title") { name = "popup-title" };
        _titleLabel.AddToClassList("popup-title");
        container.Add(_titleLabel);

        _messageLabel = new Label("Message") { name = "popup-message" };
        _messageLabel.AddToClassList("popup-message");
        container.Add(_messageLabel);

        var buttonContainer = new VisualElement { name = "popup-button-container" };
        buttonContainer.AddToClassList("popup-button-container");
        container.Add(buttonContainer);

        _okButton = new Button
        {
            text = "OK",
            name = "popup-ok"
        };
        _okButton.AddToClassList("popup-button");
        buttonContainer.Add(_okButton);

        _cancelButton = new Button(() => OnNoPressed())
        {
            text = "Nein",
            name = "popup-cancel"
        };
        _cancelButton.AddToClassList("popup-button");
        _cancelButton.style.display = DisplayStyle.None;
        buttonContainer.Add(_cancelButton);

        if (styleSheet != null)
            _popupRoot.styleSheets.Add(styleSheet);

        _popupRoot.style.display = DisplayStyle.None;
        _uiRoot.Add(_popupRoot);
    }

    private static bool EnsureInitialized()
    {
        if (_currentInstance == null)
        {
            Debug.LogError("PopupManager wurde noch nicht initialisiert! Rufe zuerst PopupManagerGeneral.Initialize(root) auf.");
            return false;
        }
        return true;
    }

    public static void Show(string title, string message)
    {
        if (!EnsureInitialized()) return;
        _currentInstance.ShowPopupInternal(title, message);
    }

    public static void ShowConfirm(string title, string message, Action onYes, Action onNo = null)
    {
        if (!EnsureInitialized()) return;
        _currentInstance.ShowConfirmInternal(title, message, onYes, onNo);
    }

    private void ShowPopupInternal(string title, string message)
    {
        _titleLabel.text = title;
        _messageLabel.text = message;
        _okButton.text = "OK";

        _okButton.clicked -= OnYesPressed;
        _okButton.clicked += HidePopup;

        _cancelButton.style.display = DisplayStyle.None;
        _popupRoot.style.display = DisplayStyle.Flex;
    }

    private void ShowConfirmInternal(string title, string message, Action onYes, Action onNo)
    {
        _titleLabel.text = title;
        _messageLabel.text = message;
        _onYesCallback = onYes;
        _onNoCallback = onNo;

        _okButton.text = "Ja";

        _okButton.clicked -= OnNoPressed;
        _okButton.clicked += OnYesPressed;

        _cancelButton.style.display = DisplayStyle.Flex;
        _popupRoot.style.display = DisplayStyle.Flex;
    }


    private void OnYesPressed()
    {
        _onYesCallback.Invoke();
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
        _okButton.clicked -= OnYesPressed;
        _okButton.clicked -= HidePopup;
        _cancelButton.style.display = DisplayStyle.None;
        _onYesCallback = null;
        _onNoCallback = null;
    }
}
