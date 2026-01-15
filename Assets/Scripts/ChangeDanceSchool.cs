using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ChangeDanceSchool : MonoBehaviour
{
    private VisualElement _mainContent;

    private void Start()
    {
        var uiDoc = FindFirstObjectByType<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("Kein UIDocument in der Szene gefunden!");
            return;
        }

        var root = uiDoc.rootVisualElement;
        _mainContent = root.Q<VisualElement>("mainContent");

        if (_mainContent == null)
        {
            Debug.LogError("mainContent nicht gefunden! Prüfe den Namen im UXML.");
            return;
        }

        _mainContent.Clear();

        var transfer = ChangeDanceSchoolDataTransfer.Instance;
        if (transfer == null)
        {
            Debug.LogError("ChangeDanceSchoolDataTransfer.Instance ist NULL!");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (transfer.DanceSchools == null || transfer.DanceSchools.Count == 0)
        {
            Debug.LogError("Keine Tanzschulen im Transfer gespeichert!");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        var heading = new Label("Wähle eine Tanzschule aus:");
        heading.AddToClassList("text-large");
        _mainContent.Add(heading);

        foreach (var school in transfer.DanceSchools)
        {
            var btn = new Button();
            btn.text = school.name;
            btn.AddToClassList("danceSchoolButton");
            btn.RemoveFromClassList("unity-button");

            btn.clicked += () =>
            {
                PlayerPrefs.SetString("locationMainMenu", "onlineDances");
                transfer.SetSelectedDanceSchoolId(school.id);
                SceneManager.LoadScene("MainMenu");
            };

            _mainContent.Add(btn);
        }
    }
}