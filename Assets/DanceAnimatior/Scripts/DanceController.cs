using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Serializable]
public class Step
{
    public int id;

    public float m1_x;
    public float m1_y;
    public bool m1_toe;
    public bool m1_heel;
    public float m1_rotate;

    public float m2_x;
    public float m2_y;
    public bool m2_toe;
    public bool m2_heel;
    public float m2_rotate;
}

[Serializable]
public class StepWrapper
{
    public Step[] steps;
}

[Serializable]
public class DanceStep
{
    public Vector3 leftFootPosition;
    public Vector3 rightFootPosition;

    public float leftRotation;
    public float rightRotation;

    public bool leftToe;
    public bool leftHeel;
    public bool rightToe;
    public bool rightHeel;
}

public class DanceController : MonoBehaviour
{
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;

    private GameObject _leftFootInstance;
    private GameObject _rightFootInstance;

    private DanceStep[] _danceSteps;

    private int _currentStepIndex;

    private Button _beginBtn;
    private Button _previousBtn;
    private Button _nextBtn;
    private Button _endBtn;
    private Button _playBtn;
    private Button _backToMenuBtn;
    private Button _spawnBtn;

    private Label _counter;
    private Label _danceName;
    private VisualElement _danceController;

    private bool _isPlaying = false;

    private void Awake()
    {
        StartCoroutine(LoadStepsFromServer());
    }

    private IEnumerator LoadStepsFromServer()
    {
        var url = "https://onlydance.at/api/getDanceById/"+DanceLoader.Instance.SelectedDanceId;
        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Fehler beim Laden der Steps: " + request.error);
            yield break;
        }

        var json = request.downloadHandler.text;
        var wrappedJson = "{\"steps\":" + json + "}";
        StepWrapper wrapper = JsonUtility.FromJson<StepWrapper>(wrappedJson);

        _danceSteps = new DanceStep[wrapper.steps.Length];

        for (int i = 0; i < wrapper.steps.Length; i++)
        {
            Step s = wrapper.steps[i];
            _danceSteps[i] = new DanceStep
            {
                leftFootPosition = new Vector3(s.m1_x, 0, s.m1_y),
                rightFootPosition = new Vector3(s.m2_x, 0, s.m2_y),

                leftRotation = s.m1_rotate,
                rightRotation = s.m2_rotate,

                leftToe = s.m1_toe,
                leftHeel = s.m1_heel,
                rightToe = s.m2_toe,
                rightHeel = s.m2_heel
            };
        }
    }

    public void Start()
    {
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
        _counter.text = "0/0";

        _danceName = root.Q<Label>("danceName");
        _danceName.text = DanceLoader.Instance.SelectedDance;

        _danceController = root.Q<VisualElement>("danceController");
        _danceController.style.display = DisplayStyle.None;
    }

    private void SpawnFeet()
    {
        if (_danceSteps == null || _danceSteps.Length == 0)
        {
            Debug.LogError("Keine Steps geladen!");
            return;
        }

        _spawnBtn.style.display = DisplayStyle.None;
        _danceController.style.display = DisplayStyle.Flex;

        var spawnPosition = Vector3.zero;
        _currentStepIndex = 0;

        _leftFootInstance = Instantiate(leftFootPrefab, spawnPosition + new Vector3(-0.2f, 0, 0), Quaternion.identity);
        _rightFootInstance = Instantiate(rightFootPrefab, spawnPosition + new Vector3(0.2f, 0, 0), Quaternion.identity);

        UpdateFootPositions();
    }

    private static void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void BeginStep() { if (_currentStepIndex <= 0) return; _currentStepIndex = 0; UpdateFootPositions(); }
    private void PreviousStep() { if (_currentStepIndex <= 0) return; _currentStepIndex--; UpdateFootPositions(); }
    private void NextStep() { if (_currentStepIndex >= _danceSteps.Length - 1) return; _currentStepIndex++; UpdateFootPositions(); }
    private void EndStep() { if (_currentStepIndex >= _danceSteps.Length - 1) return; _currentStepIndex = _danceSteps.Length - 1; UpdateFootPositions(); }
    private void PlayStep()
    {
        if (!_isPlaying)
        {
            if (_currentStepIndex == _danceSteps.Length)
                _currentStepIndex = 0;

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
        while (_currentStepIndex < _danceSteps.Length)
        {
            if (!_isPlaying) break;
            UpdateFootPositions();
            _currentStepIndex++;
            yield return new WaitForSeconds(1f);
        }
        _isPlaying = false;
        _playBtn.RemoveFromClassList("playBtnPause");
        _playBtn.AddToClassList("playBtnPlay");
    }

    private void UpdateFootPositions()
    {
        if (!_leftFootInstance || !_rightFootInstance || _danceSteps == null || _danceSteps.Length == 0) return;

        var step = _danceSteps[_currentStepIndex];

        _leftFootInstance.transform.localPosition = step.leftFootPosition;
        _rightFootInstance.transform.localPosition = step.rightFootPosition;
        
        // _leftFootInstance.transform.rotation = Quaternion.Euler(0, step.leftRotation, 0);
        // _rightFootInstance.transform.rotation = Quaternion.Euler(0, step.rightRotation, 0);

        _counter.text = (_currentStepIndex + 1) + "/" + _danceSteps.Length;
    }
}