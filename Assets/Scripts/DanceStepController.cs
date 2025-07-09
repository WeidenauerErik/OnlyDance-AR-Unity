using UnityEngine;

public class DanceStepController : MonoBehaviour
{
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;
    
    private GameObject leftFootInstance;
    private GameObject rightFootInstance;
    private bool startPositionSet = false;
    
    private Vector3[] danceSteps = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0.3f, 0, 0.2f),
        new Vector3(0.6f, 0, 0),
        new Vector3(0.3f, 0, -0.2f),
        new Vector3(0, 0, 0)
    };
    
    private int currentStepIndex = 0;

    public void SpawnFeet()
    {
        if (startPositionSet) return;
        Vector3 spawnPosition = new Vector3(0, 0, 0);
        
        leftFootInstance = Instantiate(leftFootPrefab, spawnPosition + new Vector3(-0.2f, 0, 0), Quaternion.identity);
        rightFootInstance = Instantiate(rightFootPrefab, spawnPosition + new Vector3(0.2f, 0, 0), Quaternion.identity);
        
        startPositionSet = true;
    }

    public void NextStep()
    {
        if (!startPositionSet || currentStepIndex >= danceSteps.Length - 1) return;
        
        currentStepIndex++;
        UpdateFootPositions();
    }

    public void PreviousStep()
    {
        if (!startPositionSet || currentStepIndex <= 0) return;
        
        currentStepIndex--;
        UpdateFootPositions();
    }

    private void UpdateFootPositions()
    {
        if (currentStepIndex % 2 == 0)
        {
            rightFootInstance.transform.localPosition = danceSteps[currentStepIndex];
        }
        else
        {
            leftFootInstance.transform.localPosition = danceSteps[currentStepIndex];
        }
    }
}