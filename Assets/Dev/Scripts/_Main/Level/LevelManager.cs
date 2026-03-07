using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using System.ComponentModel;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _currentSelectLevel = 0;
    [SerializeField] private int _maxOpenLevels = 0;

    private LevelBuilder _levelBuilder;

    private Dictionary<LevelSetting, bool> _levelsCompleted;

    private event Action<LevelSetting> onSetNewLevelSettings;
    private event Action<LevelSetting> onOpenNewLevel;

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

    public event Action onEraCompleted;

    [Inject]
    public void Construct(
        UnitSpawner unitSpawner,
        WaveManager waveManager,
        GameManager gameManager,
        LevelUpgrade levelUpgrader,
        LevelBuilder levelBuilder,
        UpgradesMenu upgradesMenu)
    {
        _levelBuilder = levelBuilder;

        gameManager.onWinLevel += OpenNewLevel;

        onSetNewLevelSettings += unitSpawner.SetLevelSettings;
        onSetNewLevelSettings += waveManager.SetLevelSettings;
        onSetNewLevelSettings += gameManager.SetLevelSettings;
        onSetNewLevelSettings += levelUpgrader.SetLevelSettings;

        onOpenNewLevel += unitSpawner.SetEraSettings;
        onOpenNewLevel += waveManager.SetEraSettings;
        onOpenNewLevel += gameManager.SetEraSettings;
        onOpenNewLevel += levelUpgrader.SetEraSettings;
        onOpenNewLevel += upgradesMenu.SetEraSettings;
    }

    private void Awake()
    {
        _levelsCompleted = new Dictionary<LevelSetting, bool>();

        for (int i = 0; i < LevelsSettings.Count; i++)
        {
            _levelsCompleted.Add(LevelsSettings[i], false);
        }

        onSetNewLevelSettings?.Invoke(LevelsSettings[_currentSelectLevel]);
        onOpenNewLevel?.Invoke(LevelsSettings[_currentSelectLevel]);
    }


    public void SetNextLevel()
    {
        int openLevelCount = _levelsCompleted.Where(a => a.Value == true).Count();
        int newIndex = Mathf.Clamp(_currentSelectLevel + 1, 0, openLevelCount);

        if (_currentSelectLevel != newIndex)
        {
            _currentSelectLevel = newIndex;

            _levelBuilder.SetNextLevel();
            onSetNewLevelSettings?.Invoke(LevelsSettings[_currentSelectLevel]);
        }
    }

    public void SetPreviousLevel()
    {
        int openLevelCount = _levelsCompleted.Where(a => a.Value == true).Count();
        int newIndex = Mathf.Clamp(_currentSelectLevel - 1, 0, openLevelCount);

        if(_currentSelectLevel != newIndex)
        {
            _currentSelectLevel = newIndex;

            _levelBuilder.SetPreviousLevel();
            onSetNewLevelSettings?.Invoke(LevelsSettings[_currentSelectLevel]);
        }
    }

    public void OpenNewLevel()
    {
        if (_currentSelectLevel != _maxOpenLevels) return;

        int newEraIndex = Mathf.Clamp(_maxOpenLevels + 1, 0, LevelsSettings.Count - 1);
        
        _levelsCompleted[LevelsSettings[_maxOpenLevels]] = true;
        _maxOpenLevels = newEraIndex;
        _currentSelectLevel = _maxOpenLevels;

        _levelBuilder.SetNextLevel();

        if (CompletedEra() == true) return;

        onOpenNewLevel?.Invoke(LevelsSettings[_maxOpenLevels]);
        onSetNewLevelSettings?.Invoke(LevelsSettings[_maxOpenLevels]);
    }

    private bool CompletedEra()
    {
        int completedLevels = _levelsCompleted.Count(l => l.Value == true);

        if (completedLevels >= LevelsSettings.Count)
        {
            _maxOpenLevels = 0;
            _currentSelectLevel = 0;

            onEraCompleted?.Invoke();

            onOpenNewLevel?.Invoke(LevelsSettings[_maxOpenLevels]);
            onSetNewLevelSettings?.Invoke(LevelsSettings[_maxOpenLevels]);
            
            Debug.LogWarning("Era Completed");

            return true;
        }

        return false;
    }
}
