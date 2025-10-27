using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUpManagerGeneral : MonoBehaviour
{
    private static PopUpManagerGeneral _instance;

    private VisualElement _popupRoot;
    private Label _titleLabel;
    private Label _messageLabel;
    private Button _okButton;
    private Button _cancelButton;

    private Action _onYesCallback;
    private Action _onNoCallback;

    private VisualElement _uiRoot;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public static void Initialize(VisualElement root)
    {
        if (_instance == null)
        {
            var go = new GameObject("PopupManager");
            _instance = go.AddComponent<PopUpManagerGeneral>();
        }

        _instance.Setup(root);
    }

    private void Setup(VisualElement root)
    {
        if (_uiRoot != null) return;
        _uiRoot = root;
        
        var styleSheet = Resources.Load<StyleSheet>("PopUp");
        if (styleSheet == null)
        {
            Debug.LogError("Popup.uss wurde nicht im Resources-Ordner gefunden!");
        }
        
        _popupRoot = new VisualElement();
        _popupRoot.name = "popup-root";
        _popupRoot.AddToClassList("popup-root");
        
        var container = new VisualElement();
        container.name = "popup-container";
        container.AddToClassList("popup-container");
        _popupRoot.Add(container);
        
        _titleLabel = new Label("Title");
        _titleLabel.name = "popup-title";
        _titleLabel.AddToClassList("popup-title");
        container.Add(_titleLabel);
        
        _messageLabel = new Label("Message");
        _messageLabel.name = "popup-message";
        _messageLabel.AddToClassList("popup-message");
        container.Add(_messageLabel);
        
        var buttonContainer = new VisualElement();
        buttonContainer.name = "popup-button-container";
        buttonContainer.AddToClassList("popup-button-container");
        container.Add(buttonContainer);
        
        _okButton = new Button(() => HidePopup()) { text = "OK" };
        _okButton.name = "popup-ok";
        _okButton.AddToClassList("popup-button");
        buttonContainer.Add(_okButton);
        
        _cancelButton = new Button(() => OnNoPressed()) { text = "Abbrechen" };
        _cancelButton.name = "popup-cancel";
        _cancelButton.AddToClassList("popup-button");
        _cancelButton.style.display = DisplayStyle.None;
        buttonContainer.Add(_cancelButton);

        if (styleSheet != null)
            _popupRoot.styleSheets.Add(styleSheet);

        _popupRoot.style.display = DisplayStyle.None;
        _uiRoot.Add(_popupRoot);
    }
    
    public static void Show(string title, string message)
    {
        if (!EnsureInitialized()) return;
        _instance.ShowPopupInternal(title, message);
    }

    public static void ShowConfirm(string title, string message, Action onYes, Action onNo = null)
    {
        if (!EnsureInitialized()) return;
        _instance.ShowConfirmInternal(title, message, onYes, onNo);
    }

    private static bool EnsureInitialized()
    {
        if (_instance == null)
        {
            Debug.LogError("PopupManager wurde noch nicht initialisiert! Rufe zuerst PopupManager.Initialize(root) auf.");
            return false;
        }
        return true;
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
        _okButton.clicked -= HidePopup;
        _okButton.clicked += OnYesPressed;

        _cancelButton.style.display = DisplayStyle.Flex;
        _popupRoot.style.display = DisplayStyle.Flex;
    }

    private void OnYesPressed()
    {
        HidePopup();
        _onYesCallback?.Invoke();
    }

    private void OnNoPressed()
    {
        HidePopup();
        _onNoCallback?.Invoke();
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