using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DanceAnimator : MonoBehaviour
{
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;

    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;
    
    [SerializeField] private ARRaycastManager raycastManager;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private GameObject _leftFootInstance;
    private GameObject _rightFootInstance;

    private Renderer _leftFootToeRenderer;
    private Renderer _leftFootHeelRenderer;
    private Renderer _rightFootToeRenderer;
    private Renderer _rightFootHeelRenderer;

    private GeneralSerializables.StepDanceAnimator[] _danceSteps;

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
        var uiDoc = FindFirstObjectByType<UIDocument>();
        var root = uiDoc.rootVisualElement;

        GeneralPopUpManager.Initialize();

        if (MainMenuDanceLoader.Instance.SelectedIsOnlineDance) StartCoroutine(LoadStepsFromServer());
        else LoadStepsFromLocalStorage();
    }

    private void LoadStepsFromLocalStorage()
    {
        GeneralSerializables.Step[] steps = MainMenuDanceDataManager.LoadDanceSteps(MainMenuDanceLoader.Instance.SelectedDanceId);
        SetDances(steps);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator LoadStepsFromServer()
    {
        var url = PlayerPrefs.GetString("url") + "/getDanceById/" + MainMenuDanceLoader.Instance.SelectedDanceId;
        using var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Fehler beim Laden der Steps: " + request.error);
            GeneralPopUpManager.ShowInfo("Fehler!", "Tanz konnte nicht geladen werden.");
            yield break;
        }

        var json = request.downloadHandler.text;
        Debug.Log("Backend Antwort: " + json);

        GeneralSerializables.DanceResponse response = JsonUtility.FromJson<GeneralSerializables.DanceResponse>(json);

        if (response == null)
        {
            Debug.LogError("Fehler: JSON konnte nicht geparst werden!");
            GeneralPopUpManager.ShowInfo("Fehler!", "Tanz konnte nicht geladen werden.");
            yield break;
        }

        if (!response.success)
        {
            Debug.LogError("Server meldet Fehler: success=false");
            GeneralPopUpManager.ShowInfo("Fehler!", "Tanz konnte nicht geladen werden.");
            yield break;
        }

        if (response.data == null || response.data.Length == 0)
        {
            Debug.LogError("Keine Steps in der Antwort gefunden!");
            GeneralPopUpManager.ShowInfo("Fehler!", "Tanz konnte nicht geladen werden.");
            yield break;
        }
        
        SetDances(response.data);
        Debug.Log($"Steps erfolgreich geladen: {_danceSteps.Length}");
    }

    public void SetDances(GeneralSerializables.Step[] steps)
    {
        _danceSteps = new GeneralSerializables.StepDanceAnimator[steps.Length];

        for (int i = 0; i < steps.Length; i++)
        {
            GeneralSerializables.Step s = steps[i];
            _danceSteps[i] = new GeneralSerializables.StepDanceAnimator
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
        var uiDoc = FindFirstObjectByType<UIDocument>();
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
        _danceName.text = MainMenuDanceLoader.Instance.SelectedDance;

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

        Vector3 spawnPosition = Vector3.zero;

        // 🎯 Versuche, die Fläche direkt unter der Kamera zu treffen:
        var screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        if (raycastManager.Raycast(screenCenter, _hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _hits[0].pose;
            spawnPosition = hitPose.position;
        }
        else
        {
            Debug.LogWarning("Keine AR-Fläche unter der Kamera gefunden! Verwende (0,0,0) als Fallback.");
        }

        _leftFootInstance = Instantiate(leftFootPrefab, spawnPosition + new Vector3(-0.2f, 0, 0), Quaternion.identity);
        _rightFootInstance = Instantiate(rightFootPrefab, spawnPosition + new Vector3(0.2f, 0, 0), Quaternion.identity);

        // Rest wie gehabt...
        _leftFootHeelRenderer = _leftFootInstance.transform.Find("leftheel").GetComponent<Renderer>();
        _leftFootToeRenderer = _leftFootInstance.transform.Find("lefttoe").GetComponent<Renderer>();
        _rightFootHeelRenderer = _rightFootInstance.transform.Find("rightheel").GetComponent<Renderer>();
        _rightFootToeRenderer = _rightFootInstance.transform.Find("righttoe").GetComponent<Renderer>();

        UpdateFootPositions();
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

    private void NextStep()
    {
        if (_currentStepIndex >= _danceSteps.Length - 1) return;
        _currentStepIndex++;
        UpdateFootPositions();
    }

    private void EndStep()
    {
        if (_currentStepIndex >= _danceSteps.Length - 1) return;
        _currentStepIndex = _danceSteps.Length - 1;
        UpdateFootPositions();
    }

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

        _leftFootHeelRenderer.material = step.leftHeel ? inactiveMaterial : activeMaterial;
        _leftFootToeRenderer.material = step.leftToe ? inactiveMaterial : activeMaterial;
        _rightFootHeelRenderer.material = step.rightHeel ? inactiveMaterial : activeMaterial;
        _rightFootToeRenderer.material = step.rightToe ? inactiveMaterial : activeMaterial;

        _leftFootInstance.transform.rotation = Quaternion.Euler(0, step.leftRotation, 0);
        _rightFootInstance.transform.rotation = Quaternion.Euler(0, step.rightRotation, 0);

        _counter.text = (_currentStepIndex + 1) + "/" + _danceSteps.Length;
    }
}
