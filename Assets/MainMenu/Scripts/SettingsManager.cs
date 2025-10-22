using GeneralScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static MainMenu.Scripts.MainMenu;

public class SettingsManager : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView)
    {
        mainView.Clear();
        mainView.Add(MainMenu.Scripts.MainMenu.CreateHeading("Settings"));

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
