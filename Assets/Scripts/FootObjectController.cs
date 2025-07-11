using System;
using UnityEngine;

public class FootObjectController : MonoBehaviour
{
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;

    private GameObject _leftFootInstance;
    private GameObject _rightFootInstance;
    private bool _startPositionSet;

    private Vector3[] _leftSteps;
    private Vector3[] _rightSteps;

    private int _currentStepIndex;

    public void Start()
    {
        try
        {
            // TODO: should be made a fetch function that gets the data for the arrays
            _leftSteps = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0.3f, 0, 0.2f),
                new Vector3(0.6f, 0, 0),
                new Vector3(0.3f, 0, -0.2f),
                new Vector3(0, 0, 0)
            };
            _rightSteps = new Vector3[]
            {
                new Vector3(0.2f, 0, 0),
                new Vector3(0.5f, 0, 0.2f),
                new Vector3(0.8f, 0, 0),
                new Vector3(0.5f, 0, -0.2f),
                new Vector3(0.2f, 0, 0)
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        if (_leftSteps.Length != _rightSteps.Length)
        {
            // TODO: should be made an error PopUp
        }
    }
    
    public void SpawnFeet()
    {
        if (_startPositionSet) return; // if the feet are already spawned
        var spawnPosition = new Vector3(0, 0, 0);
        _currentStepIndex = 0;

        _leftFootInstance = Instantiate(leftFootPrefab, spawnPosition + new Vector3(-0.2f, 0, 0), Quaternion.identity);
        _rightFootInstance = Instantiate(rightFootPrefab, spawnPosition + new Vector3(0.2f, 0, 0), Quaternion.identity);

        _startPositionSet = true;
    }

    public void NextStep()
    {
        if (!_startPositionSet || _currentStepIndex >= _leftSteps.Length - 1) return;

        _currentStepIndex++;
        UpdateFootPositions();
    }

    public void PreviousStep()
    {
        if (!_startPositionSet || _currentStepIndex <= 0) return;

        _currentStepIndex--;
        UpdateFootPositions();
    }

    private void UpdateFootPositions()
    {
        _rightFootInstance.transform.localPosition = _rightSteps[_currentStepIndex];
        _leftFootInstance.transform.localPosition = _leftSteps[_currentStepIndex];
    }
}