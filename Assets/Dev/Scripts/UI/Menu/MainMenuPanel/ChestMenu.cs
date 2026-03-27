using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using TMPro;

public class ChestMenu : Menu, IPointerClickHandler
{
    [Header("UI Links")]
    [SerializeField] private Image _chestIcon;
    [SerializeField] private float _clickSensitivity;

    [Header("Dropped Card UI Links")]
    [SerializeField] private Image _cardContainer;
    [SerializeField] private Image _cardIcon;
    [SerializeField] private TMP_Text _cardName;
    [SerializeField] private TMP_Text _cardDroppedCount;

    private LootManager _lootManager;
    private Chest _chestOpen;
    private ChestConfig _config;

    private float _lastClickTime;

    private Sequence _iconSequence;
    private Sequence _droppedCardSequence;

    private CardItem[] _droppedItems;
    private Dictionary<CardItem, int> _droppedCardDictianory = new();
    private int _currentDroppedCard;

    [Inject]
    public void Construct(LootManager lootManager)
    {
        _lootManager = lootManager;
        _chestOpen = new Chest(_lootManager.DroppedLoots);
    }

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        IsClosed = true;

        MenuManager?.OpenMenu(0);
    }


    public override void OpenMenu()
    {
        _cardContainer.enabled = false;
        _cardIcon.enabled = false;

        _cardName.text = "";
        _cardDroppedCount.text = "";

        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        MenuManager.CloseAllPanel();

        IsClosed = false;
    }

    public void OpenChest(ChestConfig chestConfig)
    {
        _currentDroppedCard = 0;
        _droppedItems = _chestOpen.GetDroppedCards(chestConfig);
        
        _chestIcon.transform.rotation = Quaternion.identity;
        _chestIcon.transform.localScale = Vector3.one;

        if (_droppedCardDictianory.Count > 0) _droppedCardDictianory.Clear();

        foreach (CardItem item in _droppedItems) 
        {
            if (_droppedCardDictianory.ContainsKey(item) == true)
                _droppedCardDictianory[item]++;
            else
                _droppedCardDictianory.Add(item, 1);
        }

        foreach (var droppedCard in _droppedCardDictianory)
        {
            for (int i = 0; i < droppedCard.Value; i++)
                _lootManager.LootHandler(droppedCard.Key);
        }

        Debug.Log($"<color=green>Dictianoty Length</color> {_droppedCardDictianory.Count}");
        Debug.Log($"<color=green>Dropped Item Count</color> {_droppedItems.Length}");

        OpenMenu();
        FillUI(chestConfig);
    }

    private void FillUI(ChestConfig config)
    {
        _config = config;
        _chestIcon.sprite = config.CloseIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("<color=red>Click on Chest</color>");

        _cardContainer.enabled = false;
        _cardIcon.enabled = false;
        _cardName.text = "";
        _cardDroppedCount.text = "";

        if (_currentDroppedCard >= _droppedCardDictianory.Count)
        {
            CloseMenu();
            return;
        }

        _chestIcon.sprite = _config.CloseIcon;
        CardItem item = _droppedCardDictianory.ElementAt(_currentDroppedCard).Key;
        _currentDroppedCard++;

        if (Time.time - _lastClickTime <= _clickSensitivity) return;

        _lastClickTime = Time.time;
        _iconSequence = DOTween.Sequence();

        _iconSequence
            .Append(
                _chestIcon.transform
                    .DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.5f)
                    .SetEase(Ease.OutBounce)
                    .SetLoops(1, LoopType.Yoyo))
                    .OnComplete(() => AudioManager.PlaySound("OpenChest"))
            .Append(
                _chestIcon.transform
                    .DOScale(Vector3.one, 0.5f)
                    .OnComplete(() =>
                    {
                        _chestIcon.sprite = _config.OpenIcon;
                        PlayDroppedCardAnimation(item);
                    }
                    ))
            .Append(
                _chestIcon.transform
                    .DOShakeRotation(0.5f, 20, 6, 50)
                    .SetEase(Ease.OutCubic)
            );
            

        Debug.Log($"<color=yellow>Drop Card</color>\n" +
            $"Card Name: {_droppedCardDictianory.ElementAt(_currentDroppedCard).Key}\n" +
            $"Card Count: {_droppedCardDictianory.ElementAt(_currentDroppedCard).Value}");
    }
    
    public void PlayDroppedCardAnimation(CardItem droppedItem)
    {
        _cardContainer.enabled = true;

        _cardIcon.sprite = droppedItem.Sprite;
        _cardIcon.enabled = true;

        _droppedCardSequence = DOTween.Sequence();

        _droppedCardSequence
            .Append(_cardContainer.transform.DOScale(1f, 0.5f).From(0f))
            .Append(
                _cardContainer.transform.DOLocalRotate(
                    new Vector3(0f, 360f, 0f), 0.5f, RotateMode.FastBeyond360)
                )
            .OnComplete(() =>
            {
                _cardDroppedCount.text = _droppedCardDictianory[droppedItem].ToString();
                _cardName.text = droppedItem.CardName;
            });

        AudioManager.PlaySound("DroppedCard");
    }
}
