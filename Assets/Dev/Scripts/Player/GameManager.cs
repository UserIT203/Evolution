using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, ILevelHandler
{
    private const float TimePerUpdateMoney = 1f;

    [Inject] private UnitSpawner _unitSpawner;

    [SerializeField] private Stat _moneyCountPerSecond;

    private bool _isPaused = true;

    private int _currentMoney;
    private float _addMoneyTime;
    private float _timer;

    private UnitBase[] _playerUnits;

    public float CurrentMoney => _currentMoney;

    public event Action<UnitBase, int> onInitializedUnit;
    public event Action<float, float> onChangeTime;
    public event Action<int> onChangeMoneyCount;

    public event Action onPlay;
    public event Action onEnd;
    public event Action onWinLevel;
    public event Action onLoseLevel;

    private void Update()
    {
        if (_isPaused == true) return;

        _timer += Time.deltaTime;

        if(_timer >= _addMoneyTime)
        {
            _timer = 0f;
            AddMoney(1);
        }

        onChangeTime?.Invoke(_addMoneyTime, _timer);
    }

    private void InitializedUnit()
    {
        int unitIndex = 1;

        foreach (var unit in _playerUnits)
        {
            if(unit.UnitConfig.IsUnlock == true)
            {
                onInitializedUnit?.Invoke(unit, unitIndex);
                unitIndex++;
            }
        }
    }

    public void AddMoney(int value)
    {
        _currentMoney += value;
        onChangeMoneyCount?.Invoke(_currentMoney);
    }

    public void RemoveMoney(int value)
    {
        _currentMoney -= value;
        onChangeMoneyCount?.Invoke(_currentMoney);
    }

    public void SpawnUnit(UnitBase unit)
    {
        _unitSpawner.SpawnUnit(this, unit.UnitConfig.UnitType);
        RemoveMoney(unit.UnitConfig.Cost);
    }

    public void Play()
    {
        InitializedUnit();
        _isPaused = false;

        _currentMoney = 0;

        _addMoneyTime = TimePerUpdateMoney / _moneyCountPerSecond.GetValue();
        _timer = _addMoneyTime;

        onPlay?.Invoke();
        onChangeMoneyCount?.Invoke(0);
    }

    public void EndGame(TowerType towerType)
    {
        if(_isPaused == true) return;

        onEnd?.Invoke();

        switch (towerType)
        {
            case TowerType.EnemyTower:
                onWinLevel?.Invoke();
                break;
            case TowerType.PlayerTower:
                onLoseLevel?.Invoke();
                break;
        }

        _isPaused = true;
    }

    public void UpgradePerMoneySecond(Modifier modifier) => _moneyCountPerSecond.AddModifier(modifier);

    public void EndGame() => onEnd?.Invoke();

    public void SetLevelSettings(LevelSetting levelSettings)
    {

    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        _moneyCountPerSecond.RemoveAllModifier();
        _playerUnits = levelSettings.PlayerUnits;
    }
}
