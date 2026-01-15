using System.Collections.Generic;
using UnityEngine;

public class ChangeDanceSchoolDataTransfer : MonoBehaviour
{
    public static ChangeDanceSchoolDataTransfer Instance;

    public List<GeneralSerializables.Dance> DanceSchools { get; private set; }
    public int SelectedDanceSchoolId { get; private set; }

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

    public void SetDanceSchools(List<GeneralSerializables.Dance> danceSchools, int selectedId)
    {
        DanceSchools = danceSchools;
        SelectedDanceSchoolId = selectedId;
    }

    public void SetSelectedDanceSchoolId(int id)
    {
        SelectedDanceSchoolId = id;
    }
}