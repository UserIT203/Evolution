using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using System.Linq;

public class CollectionMenu : Menu
{
    [System.Serializable]
    public struct RariteTexture
    {
        public Rarity Rarity;
        public Sprite Sprite;
    }


    [Header("UI Links")]
    [SerializeField] private TMP_Text _labelName;

    [SerializeField] private RariteTexture[] _rariteTexture;
    [SerializeField] private CardType[] _cardsTypes;
    [SerializeField] private CardUIView _cardViewPrefab;

    private GameObject _currentOpenContainer;
    private GlobalManager _globalManager;
    private AbilityManager _abilityManager;

    private Dictionary<string, CardUIView> _cardsDictionary = new();

    [Inject]
    public void Construct(GlobalManager globalManager, AbilityManager abilityManager)
    {
        _globalManager = globalManager;
        //_globalManager.onLevelUpUpgrade += UpdateCardInfo;
        //_globalManager.onAddNewCard += CreateCard;
    
        _abilityManager = abilityManager;
        //_abilityManager.onLevelUpAbility += UpdateCardInfo;
        //_abilityManager.onAddNewCard += CreateCard;
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
    }

    private void CreateCard(CardItem cardItem, ICollectedCard handler, int count)
    {
        if (_cardsDictionary.ContainsKey(cardItem.CardID) == true) return;

        Sprite background = _rariteTexture.First(t => t.Rarity == cardItem.Rarity).Sprite;

        CardUIView cardView = Instantiate(_cardViewPrefab);
        cardView.Initialized(handler, cardItem, background);

        if(handler is GlobalManager)
        {

        }
    }
}

[System.Serializable]
public struct CardType
{
    public string Name;
    public CardUIView CardView;
    public Transform Container;
    public PopUp Popup;
    public Button OpenButton;
}
