using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralLoadingSpinner : MonoBehaviour
{
    private static GeneralLoadingSpinner _instance;

    private VisualElement _loadingRoot;
    private Label _loadingLabel;

    private static VisualElement _uiRoot;

    private Coroutine _dotCoroutine;

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
            var go = new GameObject("LoadingManager");
            _instance = go.AddComponent<GeneralLoadingSpinner>();
        }
        
        var uiDoc = FindFirstObjectByType<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("Kein UIDocument in der Szene gefunden!");
            return;
        }

        _uiRoot = uiDoc.rootVisualElement;
        _instance.Setup();
    }

    private void Setup()
    {
        if (_loadingRoot != null)
            _loadingRoot.RemoveFromHierarchy();

        var styleSheet = Resources.Load<StyleSheet>("Loading");
        if (styleSheet == null)
        {
            Debug.LogError("Loading.uss wurde nicht im Resources-Ordner gefunden!");
        }
        
        _loadingRoot = new VisualElement();
        _loadingRoot.AddToClassList("loading-root");
        
        _loadingLabel = new Label("Loading");
        _loadingLabel.AddToClassList("loading-label");
        _loadingRoot.Add(_loadingLabel);

        if (styleSheet != null)
            _loadingRoot.styleSheets.Add(styleSheet);

        _loadingRoot.style.display = DisplayStyle.None;

        _uiRoot.Add(_loadingRoot);
    }

    public static void Show()
    {
        if (_instance == null)
        {
            Debug.LogError("LoadingManager nicht initialisiert!");
            return;
        }

        _instance._loadingRoot.style.display = DisplayStyle.Flex;
        
        if (_instance._dotCoroutine != null)
            _instance.StopCoroutine(_instance._dotCoroutine);

        _instance._dotCoroutine = _instance.StartCoroutine(_instance.AnimateDots());
    }

    public static void Hide()
    {
        if (_instance == null) return;

        _instance._loadingRoot.style.display = DisplayStyle.None;

        if (_instance._dotCoroutine != null)
        {
            _instance.StopCoroutine(_instance._dotCoroutine);
            _instance._dotCoroutine = null;
        }

        if (_instance._loadingLabel != null)
            _instance._loadingLabel.text = "Loading";
    }

    private IEnumerator AnimateDots()
    {
        var dotCount = 0;
        while (true)
        {
            dotCount = (dotCount % 3) + 1;
            _loadingLabel.text = "Loading" + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
