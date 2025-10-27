using GeneralScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsManager : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView)
    {
        mainView.Clear();
        mainView.Add(MainMenu.CreateHeading("Settings"));

        var logoutBtn = new Button();
        logoutBtn.text = "Logout";
        logoutBtn.clicked += () =>
        {
            DataManager.DeleteData();
            SceneManager.LoadScene("Authentication");
        };
        mainView.Add(logoutBtn);
    }
}