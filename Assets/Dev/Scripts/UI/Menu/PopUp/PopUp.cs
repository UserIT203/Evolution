using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PopUp : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] protected Image _icon;
    [SerializeField] protected TMP_Text _levelText;
    [SerializeField] protected TMP_Text _itemNameText;
    [SerializeField] protected TMP_Text _collectedCardText;
    [SerializeField] protected Image _fillImage;
    [SerializeField] protected Button _upgradeButton;
    [SerializeField] protected Button _closeButton;

    protected ICollectedCard _collected;
    protected CardItem _cardItem;
    protected CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(Exit);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(Exit);
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Exit();
    }

    public void Open(ICollectedCard collected, CardItem cardItem)
    {
        Debug.Log("<color=green>Open Popup</color>");

        _collected = collected;
        _cardItem = cardItem;

        _upgradeButton.onClick.AddListener(UpgradeCard);

        _canvasGroup.Show();

        FillUI();
    }

    public virtual void Exit()
    {
        _canvasGroup.Hide();

        _upgradeButton.onClick.RemoveAllListeners();
    }

    protected virtual void FillUI()
    {
        _icon.sprite = _cardItem.Sprite;

        _levelText.text = $"Уровень {_collected.GetLevel(_cardItem.CardID)}";
        _itemNameText.text = _cardItem.CardName;

        _collectedCardText.text =
            $"{_collected.GetCollectedCards(_cardItem.CardID)}/" +
            $"{_collected.GetCardsNeededForNextLevel(_cardItem.CardID)}";

        _fillImage.fillAmount = (float)_collected.GetCollectedCards(_cardItem.CardID) / (float)_collected.GetCardsNeededForNextLevel(_cardItem.CardID);
    }

    private void UpgradeCard()
    {
        if (_collected.TryUpgrade(_cardItem.CardID)) 
        {
            AudioManager.PlaySound("Upgrade");
            FillUI();
        } 
    }
}
