using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalManager : MonoBehaviour, ICollectedCard
{
    [field: SerializeField] public Stat HealthMultiplier;
    [field: SerializeField] public Stat DamageMultiplier;
    [field: SerializeField] public Stat SpeedMultiplier;

    [SerializeField] private int _coinsCount;

    private Dictionary<string, int> _currentLevels = new();
    private Dictionary<string, int> _collectedCards = new();
    private Dictionary<string, UnitUpradeCardConfig> _cardCollection = new();

    private Dictionary<string, (Modifier h, Modifier d, Modifier s)> _activeModifiers = new();

    public Action<CardItem, ICollectedCard, int> onAddNewCard;
    public Action<UnitUpradeCardConfig> onLevelUpUpgrade;

    public int CoinsCount
    {
        get { return _coinsCount; }
        set { _coinsCount += value; }
    }

    public void CollectCard(UnitUpradeCardConfig card)
    {
        if (string.IsNullOrEmpty(card.CardID))
        {
            Debug.LogError("Collected card without ID!");
            return;
        }

        // Инициализация
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

    private void UpgradeToNextLevel(string id, UnitUpradeCardConfig upgrade)
    {
        int oldLevel = _currentLevels[id];
        int newLevel = oldLevel + 1;
        _currentLevels[id] = newLevel;

        // Удаляем старые модификаторы
        if (_activeModifiers.TryGetValue(id, out var oldMods))
        {
            HealthMultiplier.RemoveModifier(oldMods.h);
            DamageMultiplier.RemoveModifier(oldMods.d);
            SpeedMultiplier.RemoveModifier(oldMods.s);
        }

        // Создаём новые
        var newH = upgrade.GetScaledModifier(upgrade.BaseHealthModifier, newLevel);
        var newD = upgrade.GetScaledModifier(upgrade.BaseDamageModifier, newLevel);
        var newS = upgrade.GetScaledModifier(upgrade.BaseSpeedModifier, newLevel);

        _activeModifiers[id] = (newH, newD, newS);

        // Применяем
        HealthMultiplier.AddModifier(newH);
        DamageMultiplier.AddModifier(newD);
        SpeedMultiplier.AddModifier(newS);

        onLevelUpUpgrade?.Invoke(upgrade);
        Debug.Log($"Upgraded '{id}' to level {newLevel}");
    }

    public void TryUpgrade(string id)
    {
        var config = _cardCollection[id];
        int currentLevel = _currentLevels[id];
        int collected = _collectedCards[id];

        if (currentLevel >= config.MaxLevel)
        {
            // Можно просто игнорировать или сбросить накопление
            return;
        }

        // Сколько нужно карт для перехода на следующий уровень?
        int needed = config.CardsRequiredPerLevel[currentLevel]; // индекс = текущий уровень

        if (collected >= needed)
        {
            _collectedCards[id] -= needed;
            // Выполняем апгрейд
            UpgradeToNextLevel(id, config);

            // Проверяем, можно ли апгрейднуть ещё раз (рекурсивно или в цикле)
            TryUpgrade(id); // на случай, если после вычета снова хватает
        }

        Debug.Log($"Current Collect Card in Manager {_collectedCards[id]}");
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
        if (value <= _coinsCount)
        {
            _coinsCount -= value;

            return true;
        }

        return false;
    }
}
