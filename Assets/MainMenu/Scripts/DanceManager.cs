using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private VisualElement _myDancesContainer;
    private Button _myDances;

    private VisualElement _onlineDancesContainer;
    private Button _onlineDances;
    private VisualElement _danceContainer;

    readonly string[] _myDanceList = { "Waltzer"};
    readonly string[] _onlineDanceList = { "Salsa", "Langsamer Walzer", "Slowfox", "Boogie", "Rumba", "Tango", "Cha Cha G", "Cha Cha", "Wiener Waltzer", "Servus"};

    void Start()
    {
        var uiDoc = FindObjectOfType<UIDocument>();
        var root = uiDoc.rootVisualElement;

        _myDancesContainer = root.Q<VisualElement>("myDancesContainer");
        _myDances = root.Q<Button>("myDances");
        _myDances.clicked += FetchMyDances;

        _onlineDancesContainer = root.Q<VisualElement>("onlineDancesContainer");
        _onlineDances = root.Q<Button>("onlineDances");
        _onlineDances.clicked += FetchOnlineDances;

        _danceContainer = root.Q<VisualElement>("dancesContainer");

        FetchMyDances();
    }

    private void FetchMyDances()
    {
        _danceContainer.Clear();
        _onlineDancesContainer.RemoveFromClassList("activeDancesContainer");
        _myDancesContainer.AddToClassList("activeDancesContainer");
        CreateDance(_myDanceList);
    }

    private void FetchOnlineDances()
    {
        _danceContainer.Clear();
        _myDancesContainer.RemoveFromClassList("activeDancesContainer");
        _onlineDancesContainer.AddToClassList("activeDancesContainer");
        CreateDance(_onlineDanceList);
    }

    private void CreateDance(string[] danceList)
    {
        foreach (var tmpDanceName in danceList)
        {
            var container = new VisualElement();
            container.AddToClassList("danceContainer");
            
            var danceNameLabel = new Label();
            danceNameLabel.text = tmpDanceName;
            danceNameLabel.AddToClassList("danceName");
            container.Add(danceNameLabel);
            
            var dancePlayBtn = new Button();
            dancePlayBtn.text = "+";
            dancePlayBtn.AddToClassList("dancePlayBtn");
            dancePlayBtn.clicked += () =>
            {
                DanceLoader.Instance.SetDanceName(tmpDanceName);
                SceneManager.LoadScene("DanceAnimator");
            };
            
            container.Add(dancePlayBtn);

            _danceContainer.Add(container);
        }
    }
}