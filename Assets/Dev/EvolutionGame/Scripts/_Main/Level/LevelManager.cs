using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LevelManager : MonoBehaviour, ISaveSystemService, IInitialized
{
    [Inject] private GlobalManager _globalManager;
    [Inject] private SceneLoader _sceneLoader;
    [Inject] private LevelData _levelData;

    [SerializeField] private int _currentSelectLevel = 0;
    [SerializeField] private int _maxOpenLevels = 0;

    private Dictionary<LevelSetting, bool> _levelsCompleted;

    private List<ILevelHandler> _registerHandlers = new();
    private GameManager _gameManager;

    private event Action<LevelSetting, bool> onSetNewLevelSettings;
    private event Action<LevelSetting, bool> onOpenNewLevel;

    public List<LevelSetting> LevelsSettings { get; set; }

    public int CurrentOpenLevels => _maxOpenLevels;
    public int MaxLevel => LevelsSettings.Count;
    public Sprite CurentLevelIcon
    {
        get 
        {
            return LevelsSettings[_currentSelectLevel].LevelIcon;
        }
    }

    public int CurrentSelectedLevel => _currentSelectLevel;

    public event Action onEraCompleted;
    public event Action onLevelCompleted;

    [Inject]
    public void Construct
        (
        WaveManager waveManager,
        GameManager gameManager,
        LevelUpgrade levelUpgrader,
        UpgradesMenu upgradesMenu
        )
    {
        gameManager.onWinLevel += OpenNewLevel;
        gameManager.onPlay += StartPlay;

        _gameManager = gameManager;

        RegisterToChange(waveManager);
        RegisterToChange(gameManager);
        RegisterToChange(levelUpgrader);
        RegisterToChange(upgradesMenu);
    }

    public void Initialized()
    {
        _levelsCompleted = new Dictionary<LevelSetting, bool>();

        for (int i = 0; i < LevelsSettings.Count; i++)
        {
            _levelsCompleted.Add(LevelsSettings[i], false);
        }
    }

    public void InitializedTutotial()
    {
        _levelsCompleted = new Dictionary<LevelSetting, bool>();

        for (int i = 0; i < LevelsSettings.Count; i++)
        {
            _levelsCompleted.Add(LevelsSettings[i], false);
        }

        onOpenNewLevel?.Invoke(LevelsSettings[_maxOpenLevels], false);
        onSetNewLevelSettings?.Invoke(LevelsSettings[_maxOpenLevels], false);
    }

    private void OnDestroy()
    {
        while (_registerHandlers.Count > 0)
        {
            UnregisterLevelHangler(_registerHandlers[0]);
        }
    }

    public void SetNextLevel()
    {
        int openLevelCount = _levelsCompleted.Where(a => a.Value == true).Count();
        int newIndex = Mathf.Clamp(_currentSelectLevel + 1, 0, openLevelCount);

        if (_currentSelectLevel != newIndex)
        {
            _currentSelectLevel = newIndex;
            onSetNewLevelSettings?.Invoke(LevelsSettings[_currentSelectLevel], true);
        }
    }

    public void SetPreviousLevel()
    {
        int openLevelCount = _levelsCompleted.Where(a => a.Value == true).Count();
        int newIndex = Mathf.Clamp(_currentSelectLevel - 1, 0, openLevelCount);

        if(_currentSelectLevel != newIndex)
        {
            _currentSelectLevel = newIndex;
            onSetNewLevelSettings?.Invoke(LevelsSettings[_currentSelectLevel], true);
        }
    }

    public void OpenNewLevel()
    {
        if (_currentSelectLevel != _maxOpenLevels) return;

        int newEraIndex = Mathf.Clamp(_maxOpenLevels + 1, 0, LevelsSettings.Count - 1);
        
        _levelsCompleted[LevelsSettings[_maxOpenLevels]] = true;
        _maxOpenLevels = newEraIndex;
        _currentSelectLevel = _maxOpenLevels;

        if (CompletedEra() == true)
            onEraCompleted?.Invoke();
        else
            onLevelCompleted?.Invoke();

        onOpenNewLevel?.Invoke(LevelsSettings[_maxOpenLevels], false);
        onSetNewLevelSettings?.Invoke(LevelsSettings[_maxOpenLevels], false);
    }

    public void RegisterToChange(ILevelHandler levelHandler)
    {
        onSetNewLevelSettings += levelHandler.SetLevelSettings;
        onOpenNewLevel += levelHandler.SetEraSettings;

        _registerHandlers.Add(levelHandler);
    }

    private void UnregisterLevelHangler(ILevelHandler levelHandler)
    {
        onSetNewLevelSettings -= levelHandler.SetLevelSettings;
        onOpenNewLevel -= levelHandler.SetEraSettings;

        _registerHandlers.Remove(levelHandler);
    }

    private bool CompletedEra()
    {
        int completedLevels = _levelsCompleted.Count(l => l.Value == true);

        if (completedLevels >= LevelsSettings.Count)
        {
            _maxOpenLevels = 0;
            _currentSelectLevel = 0;        

            return true;
        }

        return false;
    }

    public void LoadData()
    {
        for (int i = 0; i < _levelsCompleted.Values.Count; i++)
        {
            var key = _levelsCompleted.Keys.ElementAt(i);
            _levelsCompleted[key] = _levelData.OpenLevels[i];
        }

        _maxOpenLevels = _levelData.CurrentOpenLevel;
        _currentSelectLevel = 0;

        onSetNewLevelSettings?.Invoke(LevelsSettings[_maxOpenLevels], true);
        onOpenNewLevel?.Invoke(LevelsSettings[_maxOpenLevels], true);
    }

    public void SaveData(ISaveSystem saveSystem)
    {
        _levelData.OpenLevels = _levelsCompleted.Values.ToArray();
        _levelData.CurrentOpenLevel = CurrentOpenLevels;

        saveSystem.SaveDate(_levelData, "LevelData");
    }

    private void StartPlay()
    {
        List<LevelSpawnConfig> levelConfigs = new();

        foreach (var level in LevelsSettings)
            levelConfigs.Add(level.LevelOptions);

        EnviroumentArgs args = new EnviroumentArgs
            (
            _currentSelectLevel,
            levelConfigs, 
            _globalManager,
            _gameManager,
            LevelsSettings[_currentSelectLevel]
            );

        _sceneLoader.SwitchScene("GamePlayScene", LoadSceneMode.Additive, args).Forget();
    }
}
