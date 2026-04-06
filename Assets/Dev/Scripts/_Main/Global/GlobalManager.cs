using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GlobalManager : MonoBehaviour, ICollectedCard, ISaveSystemService, IInitialized
{
    [Inject] private LootManager _lootManager;
    [Inject] private GlobalData _globalData;

    [field: SerializeField] public Stat HealthMultiplier;
    [field: SerializeField] public Stat DamageMultiplier;
    [field: SerializeField] public Stat SpeedMultiplier;

    [SerializeField] private int _gemCount;

    private Dictionary<string, int> _currentLevels = new();
    private Dictionary<string, int> _collectedCards = new();
    private Dictionary<string, UnitUpradeCardConfig> _cardCollection = new();

    private Dictionary<string, (Modifier h, Modifier d, Modifier s)> _activeModifiers = new();

    public Action<CardItem, ICollectedCard, int> onAddNewCard;
    public Action<UnitUpradeCardConfig> onLevelUpUpgrade;

    public Action<int> onChangeCoin;

    public int GemCount
    {
        get { return _gemCount; }
        set 
        {
            _gemCount += value;
            onChangeCoin?.Invoke(_gemCount);
        }
    }

    public void Initialized()
    {
        onChangeCoin?.Invoke(_gemCount);
    }

    public void CollectCard(UnitUpradeCardConfig card)
    {
        if (string.IsNullOrEmpty(card.CardID))
        {
            Debug.LogError("Collected card without ID!");
            return;
        }

        if (!_cardCollection.ContainsKey(card.CardID))
        {
            _cardCollection[card.CardID] = card;
            _currentLevels[card.CardID] = 0;
            _collectedCards[card.CardID] = 0;
        }

        if (_activeModifiers.ContainsKey(card.CardID) == false)
        {
            var newH = card.GetScaledModifier(card.BaseHealthModifier, 0);
            var newD = card.GetScaledModifier(card.BaseDamageModifier, 0);
            var newS = card.GetScaledModifier(card.BaseSpeedModifier, 0);

            HealthMultiplier.AddModifier(newH);
            DamageMultiplier.AddModifier(newD);
            SpeedMultiplier.AddModifier(newS);

            _activeModifiers.Add(card.CardID, (newH, newD, newS));

            onAddNewCard?.Invoke(card, this, 0);
        }

        // Накапливаем
        _collectedCards[card.CardID]++;
        onLevelUpUpgrade?.Invoke(card);
    }

    public void UpgradeToNextLevel(string id, UnitUpradeCardConfig upgrade)
    {
        int oldLevel = _currentLevels[id];
        int newLevel = oldLevel + 1;
        _currentLevels[id] = newLevel;

        if (_activeModifiers.TryGetValue(id, out var oldMods))
        {
            HealthMultiplier.RemoveModifier(oldMods.h);
            DamageMultiplier.RemoveModifier(oldMods.d);
            SpeedMultiplier.RemoveModifier(oldMods.s);
        }

        var newH = upgrade.GetScaledModifier(upgrade.BaseHealthModifier, newLevel);
        var newD = upgrade.GetScaledModifier(upgrade.BaseDamageModifier, newLevel);
        var newS = upgrade.GetScaledModifier(upgrade.BaseSpeedModifier, newLevel);

        _activeModifiers[id] = (newH, newD, newS);

        HealthMultiplier.AddModifier(newH);
        DamageMultiplier.AddModifier(newD);
        SpeedMultiplier.AddModifier(newS);

        onLevelUpUpgrade?.Invoke(upgrade);
        Debug.Log($"Upgraded '{id}' to level {newLevel}");
    }

    public bool TryUpgrade(string id)
    {
        var config = _cardCollection[id];
        int currentLevel = _currentLevels[id];
        int collected = _collectedCards[id];

        if (currentLevel >= config.MaxLevel)
        {
            return false;
        }

        int needed = config.CardsRequiredPerLevel[currentLevel];

        if (collected >= needed)
        {
            _collectedCards[id] -= needed;
            UpgradeToNextLevel(id, config);

            return true;
        }

        return false;  
    }

    public int GetLevel(string id) => _currentLevels.GetValueOrDefault(id, 0);

    public int GetCollectedCards(string id) => _collectedCards.GetValueOrDefault(id, 0);

    public int GetCardsNeededForNextLevel(string id)
    {
        if (!_cardCollection.TryGetValue(id, out var config)) return 0;
        int level = _currentLevels.GetValueOrDefault(id, 0);
        if (level >= config.MaxLevel) return 0;
        return config.CardsRequiredPerLevel[level];
    }

    public CardItem[] GetActiveCards()
    {
        return _cardCollection.Values.ToArray();
    }

    public bool TryRemoveCoin(int value)
    {
        if (value <= _gemCount)
        {
            _gemCount -= value;

            onChangeCoin?.Invoke(_gemCount);

            AudioManager.PlaySound("Buy");
            return true;
        }

        return false;
    }

    public void LoadData()
    {
        _gemCount = _globalData.GemCount;

        foreach (var card in _globalData.CardCollection)
        {
            UnitUpradeCardConfig config = _lootManager.GetItemById(card.ID) as UnitUpradeCardConfig;

            _cardCollection.Add(card.ID, config);
            _currentLevels.Add(card.ID, card.CardLevel);
            _collectedCards.Add(card.ID, card.CollectedCardCount);

            var healthModifier = config.GetScaledModifier(config.BaseHealthModifier, card.CardLevel);
            var damageModifier = config.GetScaledModifier(config.BaseDamageModifier, card.CardLevel);
            var speedModifier = config.GetScaledModifier(config.BaseSpeedModifier, card.CardLevel);

            _activeModifiers.Add(card.ID, (healthModifier, damageModifier, speedModifier));
        }

        foreach (var modifier in _activeModifiers.Values)
        {
            HealthMultiplier.AddModifier(modifier.h);
            DamageMultiplier.AddModifier(modifier.d);
            SpeedMultiplier.AddModifier(modifier.s);
        }
    }

    public void SaveData(SaveSystem saveSystem)
    {        
        _globalData.GemCount = _gemCount;

        CardInfo[] cardInfo = new CardInfo[_collectedCards.Count];

        for (int i = 0; i < _cardCollection.Keys.Count; i++)
        {
            string id = _cardCollection.ElementAt(i).Key;

            CardInfo info = 
                new CardInfo(
                    id,
                    _collectedCards[id],
                    _currentLevels[id]
                );

            cardInfo[i] = info;
        }

        _globalData.CardCollection = cardInfo;

        saveSystem.SaveDate(_globalData, "GlobalData");
    }
}
