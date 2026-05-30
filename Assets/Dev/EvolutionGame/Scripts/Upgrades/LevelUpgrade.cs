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
        if(CurrentGameModifier == null)
            CurrentGameModifier = new GameModifier(_startModifier.Cost, _startModifier.Modifier);

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

    public void SetEraSettings(LevelSetting levelSettings, bool isLoadData)
    {
        Debug.Log("<color=yellow>Set Settings in Level U</color>");

        if (isLoadData == true) return;

        _coinsCount = 0;
        _currentUpgradeIndex = 0;
        CurrentGameModifier = new GameModifier(_startModifier.Cost, _startModifier.Modifier);

        onChangeMoney?.Invoke(_coinsCount);
    }

    public void SetLevelSettings(LevelSetting levelSettings, bool isLoadData)
    {
        if (isLoadData == true) return;

        _coinsCount = 0;
        _currentUpgradeIndex = 0;
        CurrentGameModifier = new GameModifier(_startModifier.Cost, _startModifier.Modifier);

        onChangeMoney?.Invoke(_coinsCount);
    }

    public void LoadData()
    {
        _coinsCount = _levelData.Coins;

        CurrentGameModifier = new GameModifier(_startModifier.Cost, _startModifier.Modifier);

        for (int i = 0; i < _levelData.LevelUpgradeCount; i++)
        {
            _gameManager.UpgradePerMoneySecond(CurrentGameModifier.Modifier);
            CurrentGameModifier.SetMultiple(_increasePreviousCost, _increasePreviousModifier, _currentUpgradeIndex);

            _currentUpgradeIndex++;

            Debug.Log($"Upgrade {CurrentGameModifier.Modifier.ModifierValue}");

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

    public GameModifier(int cost, Modifier modifier)
    {
        Cost = cost;
        Modifier = modifier;
    }

    public void SetMultiple(int value, float modifierValue, int currentLevel)
    {
        Cost += value * (currentLevel + 1);
        Modifier.ModifierValue += modifierValue;
    }
}
