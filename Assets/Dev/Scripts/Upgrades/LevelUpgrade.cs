using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class LevelUpgrade : MonoBehaviour, IItemHandler, ILevelHandler
{
    [Inject] private GameManager _gameManager;

    private List<GameModifier> _gameModifiers = new();

    private int _coinsCount;

    private Queue<GameModifier> _gameModifierQueue;
    private GameModifier _currentUpgrade;
    private int _currentUpgradeIndex = 0;

    public GameModifier CurrentGameModifier => _currentUpgrade;

    public event Action onUpgradeMoneyPerSecond;
    public event Action<int> onChangeMoney;

    private void Awake()
    {
        onChangeMoney?.Invoke(_coinsCount);
    }

    public void PickUp(int value)
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

    public bool UpgradeMoneyPerSecond()
    {
        if (CanUpgradeMoneyPerSecond() == false)
            return false;

        if (TryRemoveCoins(_currentUpgrade.Cost))
        {
            _gameManager.UpgradePerMoneySecond(_currentUpgrade.Modifier);
            
            if(_gameModifierQueue.Count >= 1)
                _currentUpgrade = _gameModifierQueue.Dequeue();

            _currentUpgradeIndex++;
        }

        onUpgradeMoneyPerSecond?.Invoke();

        return true;    
    }

    public bool CanUpgradeMoneyPerSecond() => _gameModifiers.Count > _currentUpgradeIndex;

    public void SetEraSettings(LevelSetting levelSettings)
    {
        _coinsCount = 0;
        _currentUpgradeIndex = 0;
        _gameModifiers = levelSettings.Modifiers.ToList();

        _gameModifierQueue = new Queue<GameModifier>();

        foreach (var modifier in _gameModifiers)
        {
            _gameModifierQueue.Enqueue(modifier);
        }

        _currentUpgrade = _gameModifierQueue.Dequeue();
    }

    public void SetLevelSettings(LevelSetting levelSettings)
    {
        
    }
}

[System.Serializable]
public struct GameModifier
{
    public int Cost;
    public Modifier Modifier;
}
