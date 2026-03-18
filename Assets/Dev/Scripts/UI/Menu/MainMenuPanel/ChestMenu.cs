using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

public class ChestMenu : Menu, IPointerClickHandler
{
    [Header("UI Links")]
    [SerializeField] private Image _chestIcon;

    private LootManager _lootManager;
    private Chest _chestOpen;
    private ChestConfig _config;
    
    private Sequence _iconSequence;

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
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        IsClosed = false;
    }

    public void OpenChest(ChestConfig chestConfig)
    {
        _droppedItems = _chestOpen.GetDroppedCards(chestConfig);

        foreach (CardItem item in _droppedItems) 
        {
            if (_droppedCardDictianory.ContainsKey(item) == true)
                _droppedCardDictianory[item]++;
            else
                _droppedCardDictianory.Add(item, 1);
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

    protected override void Initialized()
    {
        _iconSequence = DOTween.Sequence();

        _iconSequence
            .Append(
            _chestIcon.transform.DOShakeRotation(1f, 45, 10, 80)
            );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("<color=red>Click on Chest</color>");

        if (_currentDroppedCard > _droppedCardDictianory.Count - 1) return;

        _chestIcon.sprite = _config.CloseIcon;

        _iconSequence = DOTween.Sequence();

        _iconSequence
            .Append(
                _chestIcon.transform
                    .DOScale(new Vector3(0.8f, 0.8f, 0.8f), 2f)
                    .SetEase(Ease.OutBounce)
                    .SetLoops(1, LoopType.Yoyo))
            .Append(
                _chestIcon.transform
                    .DOScale(Vector3.one, 0.5f)
                    .OnComplete(() => _chestIcon.sprite = _config.OpenIcon))
            .Append(
                _chestIcon.transform
                    .DOShakeRotation(1f, 20, 6, 50)
                    .SetEase(Ease.OutCubic)
            );

        Debug.Log($"<color=yellow>Drop Card</color>\n" +
            $"Card Name: {_droppedCardDictianory.ElementAt(_currentDroppedCard).Key}\n" +
            $"Card Count: {_droppedCardDictianory.ElementAt(_currentDroppedCard).Value}");

        _currentDroppedCard++;
    }
    
}
