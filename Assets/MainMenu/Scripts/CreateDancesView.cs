using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CreateDancesView : MonoBehaviour
{
    private static void CreateDance(VisualElement mainView, string[] danceList)
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
            dancePlayBtn.AddToClassList("dancePlayBtn");
            dancePlayBtn.clicked += () =>
            {
                DanceLoader.Instance.SetDanceName(tmpDanceName);
                SceneManager.LoadScene("DanceAnimator");
            };

            container.Add(dancePlayBtn);

            mainView.Add(container);
        }
    }

    public static void SetMyDancesIntoView(VisualElement mainView)
    {
        string[] myDanceList = { "Walzer" };
        mainView.Clear();
        mainView.Add(new Label("Meine Tänze"));
        CreateDance(mainView, myDanceList);
    }

    public static void SetOnlineDancesIntoView(VisualElement mainView)
    {
        string[] onlineDanceList = { "Salsa", "Langsamer Walzer", "Slowfox", "Boogie", "Rumba", "Tango"};
        mainView.Clear();
        mainView.Add(new Label("Online Tänze"));
        CreateDance(mainView, onlineDanceList);
    }
}