using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;


public class TutorialManager : MonoBehaviour, ISaveSystemService
{
    [Inject] private LocalizationSelector _localizationSelector;
    [Inject] private SceneLoader _sceneLoader;
    [Inject] private PlayerData _playerData;
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
        _isTutorial = true;

        Debug.Log("<color=green>Set Tutorial Stage</color>");

        _currentStage = _stages[tutorialIndex];
        _currentInfoIndex = 0;
        _currentStageIndex = tutorialIndex;

        onSetTitle?.Invoke(_currentStage.StageName.GetText(_localizationSelector.CurrentLanguage));

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

            if(_currentStageIndex >= _stages.Count - 1) EndTutorial();

            return;
        }

        HideAllArrow();
            
        onSetInfo?.Invoke(_currentStage.Info[_currentInfoIndex]);
        _currentInfoIndex = Mathf.Clamp(_currentInfoIndex + 1, 0, _currentStage.Info.Count);
    }

    private void EndTutorial()
    {
        Debug.Log("End Tutorial");
        _sceneLoader.SwitchScene(1).Forget();
        SaveData(new SaveSystem());
    }

    public void LoadData()
    {
        throw new NotImplementedException();
    }

    public void SaveData(SaveSystem saveSystem)
    {
        _playerData.IsNewUser = false;

        saveSystem.SaveDate(_playerData, "PlayerData");
    }
}

[System.Serializable]
public class TutorialStage
{
    public LocalizeText StageName;
    public List<TutorialInfo> Info; 
}

[System.Serializable]
public class TutorialInfo
{
    [Header("<color=red><b>Descriptions</b></color>")]
    public LocalizeText Description;
    public bool HasArrow;
    public List<Image> Arrows;
}