using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class AbilityManager : MonoBehaviour, ICollectedCard, ISaveSystemService, IInitialized
{
    [Inject] private LootManager _lootManager;
    [Inject] private GlobalData _globalData;
    [Inject] private DesktopInput _desktopInput;

    [SerializeField] private UnitSpawner _unitSpawner;

    private Dictionary<string, int> _currentLevels = new();
    private Dictionary<string, int> _collectedCards = new();
    private Dictionary<string, Ability> _abilityCard = new();

    private bool _isPlay = false;
    private float _abilityUseTimer = 0;

    private GameManager _gameManager;
    private Ability _activeAbility;
    private AbilityContext _abilityContext;

    public Action<float, float> onAbilityTimer;
    public Action<Ability> onChangeAbility;
    public Action<CardItem, ICollectedCard, int> onAddNewCard;
    public Action<Ability> onLevelUpAbility;

    [Inject]
    public void Constract(GameManager gameManager)
    {
        _gameManager = gameManager;

        _gameManager.onPlay += StartPlay;
        _gameManager.onEnd += EndPlay;
    }

    public void Awake()
    {
        if (_desktopInput != null) _desktopInput.onPressButtonAbility += UseAbility;

        _abilityContext = new AbilityContext
        {
            EnemiesUnits = _unitSpawner.GetActiveEnemiesList(),
            PlayerUnits = _unitSpawner.GetActivePlayerUnitsList()
        };
    }

    private void Update()
    {
        if (_isPlay == false) return;

        if(_activeAbility != null)
        {
            if(_abilityUseTimer >= 0)
            {
                _abilityUseTimer -= Time.deltaTime;
                onAbilityTimer?.Invoke(_activeAbility.DelayTime, _abilityUseTimer);
            }      
        }
    }

    private void OnDestroy()
    {
        _gameManager.onPlay -= StartPlay;
        _gameManager.onEnd -= EndPlay;
    }

    public void Initialized()
    {
        if(_activeAbility != null) onChangeAbility?.Invoke(_activeAbility);
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

    public bool TryUpgrade(string id)
    {
        var config = _abilityCard[id];
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
            _currentLevels[id] = currentLevel + 1;

            onLevelUpAbility?.Invoke(_abilityCard[id]);

            return true;
        }

        return false;
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
            _abilityUseTimer = 0f;
            onChangeAbility?.Invoke(ability);
        }
    }

    public void UseAbility() 
    {
        if (_activeAbility == null) return;

        if(_abilityUseTimer <= 0)
        {
            _abilityUseTimer = _activeAbility.DelayTime;
            _activeAbility.Activated(_abilityContext, _currentLevels[_activeAbility.CardID]);
        }
    }

    public void LoadData()
    {
        foreach (var card in _globalData.AbilityCardCollection)
        {
            Ability config = _lootManager.GetItemById(card.ID) as Ability;

            _abilityCard.Add(card.ID, config);
            _currentLevels.Add(card.ID, card.CardLevel);
            _collectedCards.Add(card.ID, card.CollectedCardCount);
        }

        if (_abilityCard.ContainsKey(_globalData.ActiveAbilityID))
        {
            _activeAbility = _abilityCard[_globalData.ActiveAbilityID];
        }
    }

    public void SaveData(SaveSystem saveSystem)
    {
        CardInfo[] cardInfo = new CardInfo[_collectedCards.Count];

        for (int i = 0; i < _abilityCard.Keys.Count; i++)
        {
            string id = _abilityCard.ElementAt(i).Key;

            CardInfo info =
                new CardInfo(
                    id,
                    _collectedCards[id],
                    _currentLevels[id]
                );

            cardInfo[i] = info;
        }

        _globalData.AbilityCardCollection = cardInfo;
        _globalData.ActiveAbilityID = _activeAbility != null ? _activeAbility.CardID : string.Empty;

        saveSystem.SaveDate(_globalData, "GlobalData");
    }

    private void StartPlay()
    {
        _isPlay = true;
        _abilityUseTimer = 0f;
    }

    private void EndPlay() => _isPlay = false;
}
