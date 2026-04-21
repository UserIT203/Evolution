using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using System;
using TMPro;

public class CollectionMenu : Menu, IDisposable
{
    private const string LOCALIZATION_TABLE = "MenuLabels";
    private const string DEFAULT_ENTRY = "menuLabel.modifierText";

    [System.Serializable]
    public struct RariteTexture
    {
        public Rarity Rarity;
        public Sprite Sprite;
    }

    [Header("UI Links")]
    [SerializeField] private Button _buyModifierCardButton;
    [SerializeField] private LocalizeStringEvent _labelNameLocalizeEvent;

    [Header("Modifier UI Links")]
    [SerializeField] private TMP_Text _damageModifier;
    [SerializeField] private TMP_Text _healthModifier;
    [SerializeField] private TMP_Text _speedModifier;

    [Space(5f)]

    [SerializeField] private RariteTexture[] _rariteTexture;
    [SerializeField] private CardType[] _cardsTypes;
    [SerializeField] private CardUIView _cardViewPrefab;
    [SerializeField] private Transform _cardContainer;

    private CardUIView _currentActiveAbilityView;
    private CollectionType _currentOpenCollectionType;
    private GlobalManager _globalManager;
    private AbilityManager _abilityManager;

    private Dictionary<string, CardUIView> _cardsDictionary = new();
    private Dictionary<CollectionType, List<CardUIView>> _cardsTypeDictianoty;

    [Inject]
    public void Construct(GlobalManager globalManager, AbilityManager abilityManager)
    {
        _globalManager = globalManager;
        _globalManager.onLevelUpUpgrade += UpdateInfoIntoCard;
        _globalManager.onAddNewCard += CreateCard;
        _buyModifierCardButton.onClick.AddListener(() => _globalManager.GetOneModifierCard(100));

        _abilityManager = abilityManager;
        _abilityManager.onLevelUpAbility += UpdateInfoIntoCard;
        _abilityManager.onAddNewCard += CreateCard;

        _abilityManager.onChangeAbility += ShowEquipmentAbility;
    }

    public void Dispose()
    {
        _globalManager.onLevelUpUpgrade -= UpdateInfoIntoCard;
        _globalManager.onAddNewCard -= CreateCard;
        _buyModifierCardButton.onClick.RemoveAllListeners();

        _abilityManager.onLevelUpAbility -= UpdateInfoIntoCard;
        _abilityManager.onAddNewCard -= CreateCard;

        _abilityManager.onChangeAbility -= ShowEquipmentAbility;
    }

    private void OnDestroy()
    {
        foreach (var type in _cardsTypes)
            type.OpenButton.onClick.RemoveAllListeners();
    }

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();

        FillModifierText();
    }

    public override void Initialized()
    {
        _cardsTypeDictianoty = new Dictionary<CollectionType, List<CardUIView>>();

        _cardsTypeDictianoty.Add(CollectionType.Modifier, new List<CardUIView>());
        _cardsTypeDictianoty.Add(CollectionType.Ability, new List<CardUIView>());

        foreach (var modifierCard in _globalManager.GetActiveCards())
            CreateCard(modifierCard, _globalManager, 0);

        foreach (var abilityCard in _abilityManager.GetActiveCards())
            CreateCard(abilityCard, _abilityManager, 0);

        foreach (var type in _cardsTypes)
            type.OpenButton.onClick.AddListener(() => ShowCollectedCards(type.CollectionType));

        _labelNameLocalizeEvent.StringReference.SetReference(LOCALIZATION_TABLE, DEFAULT_ENTRY);

        ShowCollectedCards(_currentOpenCollectionType);
    }

    private void FillModifierText()
    {
        _damageModifier.text = "x" + _globalManager.DamageMultiplier.GetValue().ToString();
        _healthModifier.text = "x" + _globalManager.HealthMultiplier.GetValue().ToString();
        _speedModifier.text = "x" + _globalManager.SpeedMultiplier.GetValue().ToString();
    }

    private void ShowCollectedCards(CollectionType type)
    {
        Debug.Log($"Card Type {type}");

        if (_currentOpenCollectionType == type) return;

        bool isHideButton = type == CollectionType.Modifier ? true : false;
        _buyModifierCardButton.gameObject.SetActive(isHideButton);

        CardType cardType = _cardsTypes.First(i => i.CollectionType == type);
        _labelNameLocalizeEvent.StringReference.SetReference(LOCALIZATION_TABLE, cardType.Name);

        var openCards = _cardsTypeDictianoty[_currentOpenCollectionType];
        openCards.ForEach(c => c.gameObject.SetActive(false));

        var cards = _cardsTypeDictianoty[type];
        cards.ForEach(c => c.gameObject.SetActive(true));

        _currentOpenCollectionType = type;
    }

    private void CreateCard(CardItem cardItem, ICollectedCard handler, int count)
    {
        if (_cardsDictionary.ContainsKey(cardItem.CardID) == true) return;


        Sprite background = _rariteTexture.First(t => t.Rarity == cardItem.Rarity).Sprite;

        CardUIView cardView = Instantiate(_cardViewPrefab, _cardContainer, false);
        cardView.Initialized(handler, cardItem, background);

        CollectionType cardType = CollectionType.Modifier;

        if(handler is GlobalManager)
        {
            _cardsTypeDictianoty[CollectionType.Modifier].Add(cardView);   
            cardType = CollectionType.Modifier;
        }
        else if (handler is AbilityManager)
        {
            _cardsTypeDictianoty[CollectionType.Ability].Add(cardView);
            cardType = CollectionType.Ability;
        }

        if (_currentOpenCollectionType != cardType)
            cardView.gameObject.SetActive(false);

        CardType type = _cardsTypes.First(t => t.CollectionType == cardType);
        cardView.OnClickCard(type.Popup);

        _cardsDictionary.Add(cardItem.CardID, cardView);
    }

    private void UpdateInfoIntoCard(CardItem item)
    {
        FillModifierText();

        CardUIView cardView = _cardsDictionary[item.CardID];
        cardView.UpdateInfo();
    }

    private void ShowEquipmentAbility(Ability ability)
    {
        _currentActiveAbilityView?.UnEquipAbility();

        CardUIView cardView = _cardsDictionary[ability.CardID];
        cardView.EquipAbility();

        _currentActiveAbilityView = cardView;
    }
}

[System.Serializable]
public struct CardType
{
    public TableEntryReference Name;
    public CollectionType CollectionType;
    public PopUp Popup;
    public Button OpenButton;
}

public enum CollectionType
{
    Modifier,
    Ability
}
