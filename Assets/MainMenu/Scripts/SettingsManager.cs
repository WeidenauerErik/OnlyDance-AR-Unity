using UnityEngine;
using UnityEngine.UIElements;
using static MainMenu.Scripts.MainMenu;

public class SettingsManager : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView)
    {
        mainView.Clear();
        mainView.Add(MainMenu.Scripts.MainMenu.CreateHeading("Settings"));
    }
}
