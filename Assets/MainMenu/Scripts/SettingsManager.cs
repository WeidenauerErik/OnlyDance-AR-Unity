using UnityEngine;
using UnityEngine.UIElements;

public class SettingsManager : MonoBehaviour
{
    public static void SetSettingsIntoView(VisualElement mainView)
    {
        mainView.Clear();
        var label = new Label();
        label.text = "Einstellungen";
        mainView.Add(label);
    }
}
