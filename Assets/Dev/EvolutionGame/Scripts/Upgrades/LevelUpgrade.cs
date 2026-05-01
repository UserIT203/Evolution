using System;
using UnityEngine;
using Zenject;

public class LevelUpgrade : MonoBehaviour, IItemHandler, ILevelHandler, ISaveSystemService, IInitialized
{
    [Inject] private LevelData _levelData;
    [Inject] private GameManager _gameManager;
    [Inject] private MenuManager _menuManager;

    [SerializeField] private GameModifier _startModifier;
    [SerializeField] private int _increasePreviousCost;
    [SerializeField] private float _increasePreviousModifier;
    [SerializeField] private int _coinsCount;

    private int _currentUpgradeIndex;
    private bool _isPlay = false;

    public GameModifier CurrentGameModifier { get; private set; }
    public int LevelEarnedCoins { get; private set; }

    public event Action<GameModifier> onUpgradeMoneyPerSecond;
    public event Action<int> onChangeMoney;

    private void OnEnable()
    {
        _gameManager.onPlay += StartPlay;
        _gameManager.onEnd += EndPlay;
    }

    private void OnDisable()
    {
        _gameManager.onPlay -= StartPlay;
        _gameManager.onEnd -= EndPlay;
    }

    public void Initialized()
    {
        onChangeMoney?.Invoke(_coinsCount);
    }

    public void AddCoin(int value)
    {
        QuestBus.GetInstance().onUpdateCounter?.Invoke(QuestType.CollectMoney, value);
        _coinsCount += value;

        if (_isPlay == true)
            LevelEarnedCoins += value;

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

        _menuManager.GetUIMenu<ShopMenu>().OpenShopPopup();

        return false;
    }

    public void UpgradeMoneyPerSecond()
    {
        if (TryRemoveCoins(CurrentGameModifier.Cost))
        {
            _gameManager.UpgradePerMoneySecond(CurrentGameModifier.Modifier);
            CurrentGameModifier.SetMultiple(_increasePreviousCost, _increasePreviousModifier, _currentUpgradeIndex);
            _currentUpgradeIndex++;

            onUpgradeMoneyPerSecond?.Invoke(CurrentGameModifier);
        }
    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        Debug.Log("<color=yellow>Set Settings in Level U</color>");
        _coinsCount = 0;
        _currentUpgradeIndex = 0;
        CurrentGameModifier = _startModifier;
    }

    public void SetLevelSettings(LevelSetting levelSettings)
    {
        
    }

    public void LoadData()
    {
        Debug.Log($"<color=black>Load Data In Level U {_levelData.Coins}</color>");

        _coinsCount = _levelData.Coins;

        for (int i = 0; i < _levelData.LevelUpgradeCount; i++)
        {
            _gameManager.UpgradePerMoneySecond(CurrentGameModifier.Modifier);
            CurrentGameModifier.SetMultiple(_increasePreviousCost, _increasePreviousModifier, _currentUpgradeIndex);

            _currentUpgradeIndex++;

            onUpgradeMoneyPerSecond?.Invoke(CurrentGameModifier);
        }
    }

    public void SaveData(ISaveSystem saveSystem)
    {
        _levelData.Coins = _coinsCount;
        _levelData.LevelUpgradeCount = _currentUpgradeIndex;

        saveSystem.SaveDate(_levelData, "LevelData");
    }

    private void StartPlay() 
    {
        LevelEarnedCoins = 0;
        _isPlay = true;
    }

    private void EndPlay()
    {
        _isPlay = false;
    }
}

[System.Serializable]
public class GameModifier
{
    public int Cost;
    public Modifier Modifier;

    public void SetMultiple(int value, float modifierValue, int currentLevel)
    {
        Cost += value * (currentLevel + 1);
        Modifier.ModifierValue += modifierValue;
    }
}
