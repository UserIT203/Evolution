using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour, ICollectedCard
{
    [SerializeField] private Transform _bombExplosionPosition;
    [SerializeField] private UnitSpawner _unitSpawner;

    private Dictionary<string, int> _currentLevels = new();
    private Dictionary<string, int> _collectedCards = new();
    private Dictionary<string, Ability> _abilityCard = new();

    private Ability _activeAbility;
    private AbilityContext _abilityContext;

    public Action<Ability> onChangeAbility;
    public Action<CardItem, ICollectedCard, int> onAddNewCard;
    public Action<Ability> onLevelUpAbility; 

    public void Awake()
    {
        _abilityContext = new AbilityContext
        {
            BombAbilityPosition = _bombExplosionPosition,
            EnemiesUnits = _unitSpawner.GetActiveEnemiesList(),
            PlayerUnits = _unitSpawner.GetActivePlayerUnitsList()
        };
    }

    public void CollectedAbilityCard(Ability ability)
    {
        if (string.IsNullOrEmpty(ability.CardID))
        {
            Debug.LogError("Collected card without ID!");
            return;
        }

        if (!_abilityCard.ContainsKey(ability.CardID))
        {
            _abilityCard[ability.CardID] = ability;
            _currentLevels[ability.CardID] = 0;
            _collectedCards[ability.CardID] = 0;

            onAddNewCard?.Invoke(ability, this, 1);
        }

        if(_activeAbility == null)
            ChangeAbility(ability.CardID);

        _collectedCards[ability.CardID]++;
        onLevelUpAbility?.Invoke(ability);
    }

    public void TryUpgrade(string id)
    {
        var config = _abilityCard[id];
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
            _currentLevels[id] = currentLevel + 1;

            // Проверяем, можно ли апгрейднуть ещё раз (рекурсивно или в цикле)
            TryUpgrade(id); // на случай, если после вычета снова хватает
        }

        onLevelUpAbility?.Invoke(_abilityCard[id]);
        Debug.Log($"Current Collect Card in Manager {_collectedCards[id]}");
    }

    public int GetLevel(string id)
    {
        return _currentLevels[id];
    }

    public int GetCollectedCards(string id)
    {
        return _collectedCards[id];
    }

    public int GetCardsNeededForNextLevel(string id)
    {
        if (!_abilityCard.TryGetValue(id, out var config)) return 0;
        int level = _currentLevels.GetValueOrDefault(id, 0);
        if (level >= config.MaxLevel) return 0;
        return config.CardsRequiredPerLevel[level];
    }

    public CardItem[] GetActiveCards()
    {
        return _abilityCard.Values.ToArray();
    }

    public void ChangeAbility(string id)
    {
        if (_abilityCard.TryGetValue(id, out var ability))
        {
            _activeAbility = ability;
            onChangeAbility?.Invoke(ability);
        }
    }

    public void UseAbility() 
    {
        _activeAbility?.Activated(_abilityContext, _currentLevels[_activeAbility.CardID]);
    }
}
