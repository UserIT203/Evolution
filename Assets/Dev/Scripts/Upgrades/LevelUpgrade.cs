using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class LevelUpgrade : MonoBehaviour, IItemHandler, ILevelHandler, ISaveSystemService, IInitialized
{
    [Inject] private LevelData _levelData;
    [Inject] private GameManager _gameManager;

    [SerializeField] private GameModifier _startModifier;
    [SerializeField] private int _increasePreviousCost;
    [SerializeField] private float _increasePreviousModifier;
    [SerializeField] private int _coinsCount;

    private int _currentUpgradeIndex;

    public GameModifier CurrentGameModifier { get; private set; }

    public event Action<GameModifier> onUpgradeMoneyPerSecond;
    public event Action<int> onChangeMoney;

    public void Initialized()
    {
        onChangeMoney?.Invoke(_coinsCount);
    }

    public void AddCoin(int value)
    {
        _coinsCount += value;
        onChangeMoney?.Invoke(_coinsCount);
    }

    public bool TryRemoveCoins(int value)
    {
        if(_coinsCount >= value)
        {
            _coinsCount -= value;
            onChangeMoney?.Invoke(_coinsCount);

            return true;
        }

        return false;
    }

    public void UpgradeMoneyPerSecond()
    {
        if (TryRemoveCoins(CurrentGameModifier.Cost))
        {
            _gameManager.UpgradePerMoneySecond(CurrentGameModifier.Modifier);
            CurrentGameModifier.SetMultiple(_increasePreviousCost, _increasePreviousModifier);
            _currentUpgradeIndex++;

            onUpgradeMoneyPerSecond?.Invoke(CurrentGameModifier);
        }
    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        _coinsCount = 0;
        _currentUpgradeIndex = 1;
        CurrentGameModifier = _startModifier;
    }

    public void SetLevelSettings(LevelSetting levelSettings)
    {
        
    }

    public void LoadData()
    {
        _coinsCount = _levelData.Coins;

        for (int i = 0; i < _levelData.LevelUpgradeCount; i++)
        {
            _gameManager.UpgradePerMoneySecond(CurrentGameModifier.Modifier);
            CurrentGameModifier.SetMultiple(_increasePreviousCost, _increasePreviousModifier);
            _currentUpgradeIndex++;

            onUpgradeMoneyPerSecond?.Invoke(CurrentGameModifier);
        }
    }

    public void SaveData(SaveSystem saveSystem)
    {
        _levelData.Coins = _coinsCount;
        _levelData.LevelUpgradeCount = _currentUpgradeIndex;

        saveSystem.SaveDate(_levelData, "LevelData");
    }
}

[System.Serializable]
public class GameModifier
{
    public int Cost;
    public Modifier Modifier;

    public void SetMultiple(int value, float modifierValue)
    {
        Cost *= value;
        Modifier.ModifierValue += modifierValue;
    }
}
