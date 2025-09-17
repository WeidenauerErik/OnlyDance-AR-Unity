using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DanceController : MonoBehaviour
{
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;

    private GameObject _leftFootInstance;
    private GameObject _rightFootInstance;

    private Vector3[] _leftSteps;
    private Vector3[] _rightSteps;

    private int _currentStepIndex;

    private Button _beginBtn;
    private Button _previousBtn;
    private Button _nextBtn;
    private Button _endBtn;
    private Button _playBtn;
    private Image _playBtnImage;

    private Button _backToMenuBtn;
    private Button _spawnBtn;

    private Label _counter;
    private Label _danceName;

    private VisualElement _danceController;
    
    public void Start()
    {
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

        var uiDoc = FindObjectOfType<UIDocument>();
        var root = uiDoc.rootVisualElement;

        _spawnBtn = root.Q<Button>("spawnBtn");
        _spawnBtn.clicked += SpawnFeet;

        _backToMenuBtn = root.Q<Button>("backToMenuBtn");
        _backToMenuBtn.clicked += BackToMenu;

        _beginBtn = root.Q<Button>("beginBtn");
        _beginBtn.clicked += BeginStep;

        _previousBtn = root.Q<Button>("previousBtn");
        _previousBtn.clicked += PreviousStep;

        _playBtn = root.Q<Button>("playBtn");
        _playBtn.clicked += PlayStep;

        _nextBtn = root.Q<Button>("nextBtn");
        _nextBtn.clicked += NextStep;

        _endBtn = root.Q<Button>("endBtn");
        _endBtn.clicked += EndStep;

        _counter = root.Q<Label>("counter");
        _counter.text = "1/" + _leftSteps.Length;

        _danceName = root.Q<Label>("danceName");
        _danceName.text = DanceLoader.Instance.SelectedDance;

        _danceController = root.Q<VisualElement>("danceController");
        _danceController.style.display = DisplayStyle.None;
    }

    private void SpawnFeet()
    {
        _spawnBtn.style.display = DisplayStyle.None;
        _danceController.style.display = DisplayStyle.Flex;
        var spawnPosition = new Vector3(0,0,0);

        _currentStepIndex = 0;

        _leftFootInstance = Instantiate(leftFootPrefab, spawnPosition + new Vector3(-0.2f, 0, 0), Quaternion.identity);
        _rightFootInstance = Instantiate(rightFootPrefab, spawnPosition + new Vector3(0.2f, 0, 0), Quaternion.identity);
    }

    private static void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void BeginStep()
    {
        if (_currentStepIndex <= 0) return;

        _currentStepIndex = 0;
        UpdateFootPositions();
    }

    private void PreviousStep()
    {
        if (_currentStepIndex <= 0) return;

        _currentStepIndex--;
        UpdateFootPositions();
    }

    private bool _isPlaying = false;

    private void PlayStep()
    {
        if (!_isPlaying)
        {
            if (_currentStepIndex == _leftSteps.Length)
            {
                _currentStepIndex = 0;
            }
            _playBtn.RemoveFromClassList("playBtnPlay");
            _playBtn.AddToClassList("playBtnPause");
            StartCoroutine(PlayDanceRoutine());
        }
        else
        {
            _isPlaying = false;
            _playBtn.RemoveFromClassList("playBtnPause");
            _playBtn.AddToClassList("playBtnPlay");
        }
    }

    private IEnumerator PlayDanceRoutine()
    {
        _isPlaying = true;

        while (_currentStepIndex < _leftSteps.Length)
        {
            if (!_isPlaying)
                break;

            UpdateFootPositions();
            _currentStepIndex++;

            yield return new WaitForSeconds(1f);
        }

        _isPlaying = false;
        _playBtn.RemoveFromClassList("playBtnPause");
        _playBtn.AddToClassList("playBtnPlay");
    }


    private void NextStep()
    {
        if (_currentStepIndex >= _leftSteps.Length - 1) return;

        _currentStepIndex++;
        UpdateFootPositions();
    }

    private void EndStep()
    {
        if (_currentStepIndex >= _leftSteps.Length - 1) return;

        _currentStepIndex = _leftSteps.Length - 1;
        UpdateFootPositions();
    }

    private void UpdateFootPositions()
    {
        _rightFootInstance.transform.localPosition = _rightSteps[_currentStepIndex];
        _leftFootInstance.transform.localPosition = _leftSteps[_currentStepIndex];
        _counter.text = (_currentStepIndex + 1) + "/" + _leftSteps.Length;
    }
}