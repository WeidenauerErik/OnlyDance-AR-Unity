using UnityEngine;

public class MainMenuDanceDataTransfer : MonoBehaviour
{
    public static MainMenuDanceDataTransfer Instance;

    public string SelectedDance { get; private set; }
    public int SelectedDanceId { get; private set; }
    public bool SelectedIsOnlineDance { get; private set; }

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

    public void SetDanceCredentials(string danceName, int danceId, bool isOnlineDance)
    {
        SelectedDance = danceName;
        SelectedDanceId = danceId;
        SelectedIsOnlineDance = isOnlineDance;
    }
}