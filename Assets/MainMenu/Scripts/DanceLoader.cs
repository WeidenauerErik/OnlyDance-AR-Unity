using UnityEngine;

public class DanceLoader : MonoBehaviour
{
    public static DanceLoader Instance;

    public string SelectedDance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDance(string danceName)
    {
        SelectedDance = danceName;
    }
}