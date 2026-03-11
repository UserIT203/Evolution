using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardUIView : MonoBehaviour
{
    [SerializeField] private Image _equipIcon;
    [SerializeField] protected Image _icon;
    [SerializeField] protected Image _background;
    [SerializeField] protected TMP_Text _currentLevelText;
    [SerializeField] protected TMP_Text _collectedCardText;

    private Button _button;

    protected ICollectedCard _collectManager;
    protected CardItem _card;

    public Action<CardItem, ICollectedCard> onClickCard;

    private void Awake()
    {
        UnEquipAbility();
        _button = GetComponent<Button>();
    }

    public void Initialized(
        ICollectedCard collectManager, 
        CardItem card, 
        Sprite backgroundSprite)
    {
        _background.sprite = backgroundSprite;
        _collectManager = collectManager;
        _card = card;
    }

    public virtual void UpdateInfo()
    {
        _icon.sprite = _card.Sprite;
        _currentLevelText.text = _collectManager.GetLevel(_card.CardID).ToString();

        _collectedCardText.text = string.Format($"{_collectManager.GetCollectedCards(_card.CardID)} | {_collectManager.GetCardsNeededForNextLevel(_card.CardID)}");
    }

    public void EquipAbility() => _equipIcon.enabled = true;
    public void UnEquipAbility() => _equipIcon.enabled = false;

    public void OnClickCard(Action clickEvent)
    {
        _button.onClick.AddListener(() => clickEvent?.Invoke());
    }
}
