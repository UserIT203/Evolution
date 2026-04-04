using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;


public class TutorialManager : MonoBehaviour
{
    [Inject] private GameManager _gameManager;

    [SerializeField] private List<TutorialStage> _stages;

    private bool _isTutorial;
    private int _currentInfoIndex;
    private int _currentStageIndex;

    private List<Image> _arrowList = new();
    private TutorialStage _currentStage;

    public Action<TutorialInfo> onSetInfo;
    public Action<string> onSetTitle;
    public Action onEndStage;

    private void OnEnable()
    {
        _gameManager.onPlay += () => SetTutorialState(1);
        _gameManager.onEnd += () => SetTutorialState(2);
    }

    private void OnDisable()
    {
        _gameManager.onPlay -= () => SetTutorialState(1);
        _gameManager.onEnd -= () => SetTutorialState(2);
    }

    private void Awake()
    {
        foreach (var stage in _stages)
        {
            foreach (var info in stage.Info)
            {
                _arrowList.AddRange(info.Arrows);
            }
        }

        HideAllArrow();
    }

    private void Start()
    {
        SetTutorialState(0);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame == true)
        {
            Debug.Log("<color=red>Click</color>");
            SetDescription();
        }
    }

    private void HideAllArrow()
    {
        _arrowList.ForEach(i => i.enabled = false);
    }

    private void SetTutorialState(int tutorialIndex)
    {
        if (tutorialIndex >= _stages.Count)
        {
            EndTutorial();
            return;
        }

        _isTutorial = true;

        Debug.Log("<color=green>Set Tutorial Stage</color>");

        _currentStage = _stages[tutorialIndex];
        _currentInfoIndex = 0;
        _currentStageIndex = tutorialIndex;

        onSetTitle?.Invoke(_currentStage.StageName);

        SetDescription();
        Time.timeScale = 0f;
    }

    private void SetDescription()
    {
        if (_currentInfoIndex >= _currentStage.Info.Count && _isTutorial == true)
        {
            Debug.Log("<color=red>End Tutorial Stage</color>");
            Time.timeScale = 1f;
            _isTutorial = false;
            
            onEndStage?.Invoke();
        }

        HideAllArrow();
            
        onSetInfo?.Invoke(_currentStage.Info[_currentInfoIndex]);
        _currentInfoIndex = Mathf.Clamp(_currentInfoIndex + 1, 0, _currentStage.Info.Count);
    }

    private void EndTutorial()
    {

    }
}

[System.Serializable]
public class TutorialStage
{
    public string StageName;
    public List<TutorialInfo> Info; 
}

[System.Serializable]
public class TutorialInfo
{
    [Header("<color=red><b>Descriptions</b></color>")]
    [TextArea(2, 10)] public string Description;
    public bool HasArrow;
    public List<Image> Arrows;
}