using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsManagerMainMenu : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView)
    {
        mainView.Clear();
        mainView.Add(MainMenu.CreateHeading("Settings"));
        
        var data = DataManagerGeneral.LoadData();
        var emailLabel = new Label { text = data.email };
        emailLabel.AddToClassList("settings-emailLabel");
        
        var emailContainer = new VisualElement();
        emailContainer.AddToClassList("settings-emailContainer");
        emailContainer.Add(emailLabel);
        mainView.Add(emailContainer);
        
        var logoutBtn = new Button { text = "Logout" };
        logoutBtn.AddToClassList("settings-buttons");
        logoutBtn.clicked += () =>
        {
            PopUpManagerGeneral.ShowConfirm("Bist du dir sicher?", "Willst du dich wirklich abmelden?", () =>
            {
                Debug.Log("Logged out");
                DataManagerGeneral.DeleteData();
                SceneManager.LoadScene("Authentication");
                
            });
        };
        mainView.Add(logoutBtn);
    }
}