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
            DontDestroyOnLoad(gameObject);  //if you switch scenes the object won't be destroyed
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDanceName(string danceName)
    {
        SelectedDance = danceName;
    }
}