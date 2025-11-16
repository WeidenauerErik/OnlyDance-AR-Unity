using UnityEngine;


public class MainMenuDanceLoader : MonoBehaviour
{
    public static MainMenuDanceLoader Instance;

    public string SelectedDance { get; private set; }
    public int SelectedDanceId { get; private set; }
    public bool SelectedIsOnlineDance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //if you switch scenes the object won't be destroyed
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDanceCredentials(string danceName, int danceId, bool IsOnlineDance)
    {
        SelectedDance = danceName;
        SelectedDanceId = danceId;
        SelectedIsOnlineDance = IsOnlineDance;
    }
}