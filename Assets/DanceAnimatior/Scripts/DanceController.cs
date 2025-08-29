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

    private Button beginBtn;
    private Button previousBtn;
    private Button playBtn;
    private Button nextBtn;
    private Button endBtn;
    
    private Button backToMenuBtn;
    private Button spawnBtn;

    private Label counter;
    private Label danceName;

    private VisualElement danceController;

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

        spawnBtn = root.Q<Button>("spawnBtn");
        spawnBtn.clicked += SpawnFeet;
        
        backToMenuBtn = root.Q<Button>("backToMenuBtn");
        backToMenuBtn.clicked += backToMenu;

        beginBtn = root.Q<Button>("beginBtn");
        beginBtn.clicked += BeginStep;

        previousBtn = root.Q<Button>("previousBtn");
        previousBtn.clicked += PreviousStep;

        playBtn = root.Q<Button>("playBtn");
        playBtn.clicked += PlayStep;

        nextBtn = root.Q<Button>("nextBtn");
        nextBtn.clicked += NextStep;

        endBtn = root.Q<Button>("endBtn");
        endBtn.clicked += EndStep;

        counter = root.Q<Label>("counter");
        counter.text = "1/" + _leftSteps.Length;

        danceName = root.Q<Label>("danceName");
        danceName.text = DanceLoader.Instance.SelectedDance;
        
        danceController = root.Q<VisualElement>("danceController");
        danceController.style.display = DisplayStyle.None;
    }

    private void SpawnFeet()
    {
        spawnBtn.style.display = DisplayStyle.None;
        danceController.style.display = DisplayStyle.Flex;
        var spawnPosition = new Vector3(0, 0, 0);
        _currentStepIndex = 0;

        _leftFootInstance = Instantiate(leftFootPrefab, spawnPosition + new Vector3(-0.2f, 0, 0), Quaternion.identity);
        _rightFootInstance = Instantiate(rightFootPrefab, spawnPosition + new Vector3(0.2f, 0, 0), Quaternion.identity);
    }

    private void backToMenu()
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

    private bool isPlaying = false;

    public void PlayStep()
    {
        if (!isPlaying)
            StartCoroutine(PlayDanceRoutine());
    }

    private IEnumerator PlayDanceRoutine()
    {
        isPlaying = true;

        while (_currentStepIndex < _leftSteps.Length)
        {
            UpdateFootPositions();
            _currentStepIndex++;

            yield return new WaitForSeconds(1f);
        }

        isPlaying = false;
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
        counter.text = (_currentStepIndex + 1) + "/" + _leftSteps.Length;
    }
}