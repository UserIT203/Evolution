using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CardsMenu : Menu
{
    [SerializeField] private CardType[] _cardsTypes;

    [Header("UI Links")]
    [SerializeField] private CardPop _cardPopup;

    private Dictionary<string, CardUIView> _cards;

    private GameObject _currentOpenContainer;
    private GlobalManager _globalManager;
    private AbilityManager _abilityManager;

    [Inject]
    public void Construct(GlobalManager globalManager, AbilityManager abilityManager)
    {
        _globalManager = globalManager;
        _globalManager.onLevelUpUpgrade += UpdateCardInfo;
        _globalManager.onAddNewCard += CreateCard;
    
        _abilityManager = abilityManager;
        _abilityManager.onLevelUpAbility += UpdateCardInfo;
        _abilityManager.onAddNewCard += CreateCard;
    }

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    protected override void Initialized()
    {
        _currentOpenContainer = _cardsTypes[0].Container.gameObject;

        int cardTypeIndex = 0;
        foreach (var cardType in _cardsTypes)
        {
            cardType.OpenButton.onClick.AddListener(() => OpenCardsView(cardType.Container.gameObject));
            cardTypeIndex++;
        }

        _cards = new Dictionary<string, CardUIView>();

        foreach (var card in _globalManager.GetActiveCards())
            CreateCard(card, _globalManager, 0);

        foreach (var card in _abilityManager.GetActiveCards()) 
            CreateCard(card, _abilityManager, 1);

    }

    private void CreateCard(CardItem item, ICollectedCard collectCard, int typeIndex)
    {
        CardUIView cardView = Instantiate(_cardsTypes[typeIndex].CardView);
        cardView.transform.SetParent(_cardsTypes[typeIndex].Container, false);
        cardView.Initialized(collectCard, item);

        cardView.onClickCard += OpenPopup;

        _cards.Add(item.CardID, cardView);
    }

    private void UpdateCardInfo(CardItem card)
    {
        _cards[card.CardID].UpdateInfo();
    }

    private void OpenCardsView(GameObject container)
    {
        if(_currentOpenContainer != null)
            _currentOpenContainer.gameObject.SetActive(false);

        container.gameObject.SetActive(true);
        _currentOpenContainer = container;
    }

    private void OpenPopup(CardItem card, ICollectedCard collectedCard)
    {
        _cardPopup.OpenPopUp(card, collectedCard);
    }
}

[System.Serializable]
public struct CardType
{
    public CardUIView CardView;
    public Transform Container;
    public Button OpenButton;
}
