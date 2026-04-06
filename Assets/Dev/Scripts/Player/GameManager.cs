using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, ILevelHandler, ISaveSystemService
{
    [Inject] private LevelData _levelData;

    private const float TimePerUpdateMoney = 1f;

    [Inject] private DesktopInput _desktopInput;
    [Inject] private UnitSpawner _unitSpawner;

    [SerializeField] private Stat _moneyCountPerSecond;

    private bool _isPaused = true;

    private int _currentMoney;
    private float _addMoneyTime;
    private float _timer;

    [field: SerializeField] public UnitInfo[] UnitInfo { get; private set; }
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

        foreach (var unit in UnitInfo)
        {
            if(unit.IsUnlock == true)
            {
                _playerUnitsActived.Add(unitIndex, unit.Unit);
                onInitializedUnit?.Invoke(unit.Unit, unitIndex);
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
        _moneyCountPerSecond.RemoveAllModifier();
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

        UnitInfo = new UnitInfo[levelSettings.PlayerUnits.Length];

        for (int i = 0; i < levelSettings.PlayerUnits.Length; i++)
        {
            UnitInfo info = new UnitInfo(levelSettings.PlayerUnits[i]);
            UnitInfo[i] = info;
        }
    }

    public void LoadData()
    {
        for (int i = 0; i < _levelData.UnlockUnits.Length; i++)
            UnitInfo[i].IsUnlock = _levelData.UnlockUnits[i];
    }

    public void SaveData(SaveSystem saveSystem)
    {
        bool[] unlockUnit = new bool[UnitInfo.Length];

        for(int i = 0;i < UnitInfo.Length; i++)
        {
            unlockUnit[i] = UnitInfo[i].IsUnlock;
        }

        _levelData.UnlockUnits = unlockUnit;

        saveSystem.SaveDate(_levelData, "LevelData");
    }
}

[System.Serializable]
public class UnitInfo
{
    public UnitBase Unit { get; set; }
    public bool IsUnlock { get; set; }

    public UnitInfo(UnitBase unit) 
    { 
        Unit = unit;
        IsUnlock = unit.UnitConfig.IsUnlock;
    }
}