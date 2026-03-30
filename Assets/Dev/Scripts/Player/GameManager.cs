using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, ILevelHandler
{
    private const float TimePerUpdateMoney = 1f;

    [Inject] private DesktopInput _desktopInput;
    [Inject] private UnitSpawner _unitSpawner;

    [SerializeField] private Stat _moneyCountPerSecond;

    private bool _isPaused = true;

    private int _currentMoney;
    private float _addMoneyTime;
    private float _timer;

    private UnitBase[] _playerUnits;

    public float CurrentMoney => _currentMoney;

    private Dictionary<int, UnitBase> _playerUnitsActived = new(); 

    public event Action<UnitBase, int> onInitializedUnit;
    public event Action<float, float> onChangeTime;
    public event Action<int> onChangeMoneyCount;

    public event Action onPlay;
    public event Action onEnd;
    public event Action onWinLevel;
    public event Action onLoseLevel;

    private void Awake()
    {
        if (_desktopInput != null) _desktopInput.onGetPressButtonIndex += SpawnUnitFromDesktopInput;
    }

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
        if (_playerUnitsActived.Count > 0) _playerUnitsActived.Clear();

        int unitIndex = 1;

        foreach (var unit in _playerUnits)
        {
            if(unit.UnitConfig.IsUnlock == true)
            {
                _playerUnitsActived.Add(unitIndex, unit);
                onInitializedUnit?.Invoke(unit, unitIndex);
                unitIndex++;
            }
        }
    }

    private void SpawnUnitFromDesktopInput(int index)
    {
        UnitBase spawnUnit = _playerUnitsActived[index];

        if (_currentMoney >= spawnUnit.UnitConfig.Cost)
            SpawnUnit(spawnUnit);
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
        AudioManager.PlaySound("SpawnUnit");
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

        _desktopInput?.EnableInput();

        AudioManager.Instance.SetAmbient("BattleAmbient");

        onPlay?.Invoke();
        onChangeMoneyCount?.Invoke(0);
    }

    public void EndGame(TowerType towerType)
    {
        AudioManager.Instance.SetAmbient("MainAmbient");

        if (_isPaused == true) return;

        _desktopInput?.DisableInput();

        onEnd?.Invoke();

        switch (towerType)
        {
            case TowerType.EnemyTower:
                AudioManager.PlaySound("Win");
                onWinLevel?.Invoke();
                break;
            case TowerType.PlayerTower:
                AudioManager.PlaySound("Lose");
                onLoseLevel?.Invoke();
                break;
        }

        _isPaused = true; 
    }

    public void UpgradePerMoneySecond(Modifier modifier)
    {
        _moneyCountPerSecond.AddModifier(modifier);
    }

    public void EndGame()
    {
        AudioManager.Instance.SetAmbient("MainAmbient");

        onEnd?.Invoke();
    }

    public void SetLevelSettings(LevelSetting levelSettings)
    {

    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        _moneyCountPerSecond.RemoveAllModifier();
        _playerUnits = levelSettings.PlayerUnits;
    }
}
