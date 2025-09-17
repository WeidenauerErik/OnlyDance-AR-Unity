using UnityEngine;
using UnityEngine.UIElements;
using static TabSwitcher;

public class SettingsManager : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView)
    {
        mainView.Clear();
        mainView.Add(TabSwitcher.CreateHeading("Settings"));
    }
}
