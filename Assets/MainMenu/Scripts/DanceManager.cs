using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private VisualElement myDancesContainer;
    private Button myDances;
    
    private VisualElement onlineDancesContainer;
    private Button onlineDances;
    private ScrollView danceContainer;
    
    string[] myDanceList = { "Waltz", "Tango", "HipHop" };
    string[] onlineDanceList = { "Salsa", "Bachata", "Charleston" };

    void Start()
    {
        var uiDoc = FindObjectOfType<UIDocument>();
        var root = uiDoc.rootVisualElement;
        
        myDancesContainer = root.Q<VisualElement>("myDancesContainer");
        myDances = root.Q<Button>("myDances");
        myDances.clicked += FetchMyDances;
        
        onlineDancesContainer = root.Q<VisualElement>("onlineDancesContainer");
        onlineDances = root.Q<Button>("onlineDances");
        onlineDances.clicked += FetchOnlineDances;
        
        danceContainer = root.Q<ScrollView>("danceContainer");
        
        FetchMyDances();
    }

    private void FetchMyDances()
    {
        danceContainer.Clear();
        onlineDancesContainer.RemoveFromClassList("activeDancesContainer");
        myDancesContainer.AddToClassList("activeDancesContainer");
        
        foreach (var dance in myDanceList)
        {
            var btn = new Button();
            btn.text = dance;
            
            string selectedDance = dance;

            btn.clicked += () =>
            {
                DanceLoader.Instance.SetDance(selectedDance);
                SceneManager.LoadScene("DanceAnimator");
            };

            danceContainer.Add(btn);
        }
    }

    private void FetchOnlineDances()
    {
        danceContainer.Clear();
        myDancesContainer.RemoveFromClassList("activeDancesContainer");
        onlineDancesContainer.AddToClassList("activeDancesContainer");
        
        foreach (var dance in onlineDanceList)
        {
            var btn = new Button();
            btn.text = dance;
            string selectedDance = dance;

            btn.clicked += () =>
            {
                DanceLoader.Instance.SetDance(selectedDance);
                SceneManager.LoadScene("DanceAnimator");
            };

            danceContainer.Add(btn);
        }
    }
}