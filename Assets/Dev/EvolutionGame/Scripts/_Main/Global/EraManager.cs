using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EraManager : MonoBehaviour
{
    [SerializeField] private List<EraSettings> _eraSettings;

    private int _currentEra;

    private List<LevelSpawnConfig> _currentLevelSpawnConfigs;

    private LevelManager _levelManager;

    public int CurrentEra => _currentEra;

    [Inject]
    public void Construct(LevelManager levelManager)
    {
        _levelManager = levelManager;

        _levelManager.onEraCompleted += TrySetNewEra;

        SetNewEraConfig();
    }

    private void OnDestroy()
    {
        _levelManager.onEraCompleted -= TrySetNewEra;
    }

    private void SetNewEraConfig()
    {
        _levelManager.LevelsSettings = _eraSettings[_currentEra].EraLevelsSettings;

        _currentLevelSpawnConfigs = new List<LevelSpawnConfig>();

        foreach (var level in _eraSettings[_currentEra].EraLevelsSettings)
        {
            _currentLevelSpawnConfigs.Add(level.LevelOptions);
        }
    }

    private void TrySetNewEra()
    {
        int newEraIndex = Mathf.Clamp(_currentEra + 1, 0, _eraSettings.Count);

        if(newEraIndex == _eraSettings.Count - 1)
        {
            Debug.LogWarning("<color=green>Game Completed</color>");
        }

        if(_currentEra != newEraIndex)
        {
            _eraSettings[_currentEra].IsCompleted = true;
            _currentEra = newEraIndex;

            SetNewEraConfig();
        }
    }
}

[System.Serializable]
public class EraSettings
{
    public int EraId;
    public List<LevelSetting> EraLevelsSettings;
    public bool IsCompleted;
}
