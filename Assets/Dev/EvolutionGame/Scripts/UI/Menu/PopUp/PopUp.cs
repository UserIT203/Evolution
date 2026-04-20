using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using Unity.VisualScripting;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PopUp : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] protected Image _icon;
    [SerializeField] protected LocalizeStringEvent _levelTextLocalizeEvent;
    [SerializeField] protected TMP_Text _itemNameText;
    [SerializeField] protected TMP_Text _collectedCardText;
    [SerializeField] protected Image _fillImage;
    [SerializeField] protected Button _upgradeButton;
    [SerializeField] protected Button _closeButton;

    protected LocalizationSelector _localizationSelector;

    protected ICollectedCard _collected;
    protected CardItem _cardItem;
    protected CanvasGroup _canvasGroup;

    [Inject]
    public void Construct(LocalizationSelector selector)
    {
        _localizationSelector = selector;
        _localizationSelector.onChangeLocale += UpdateLocaleText;
    }

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

    private void OnDestroy()
    {
        _localizationSelector.onChangeLocale -= UpdateLocaleText;
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

        if (_levelTextLocalizeEvent.StringReference.Arguments == null)
            _levelTextLocalizeEvent.StringReference.Arguments = new object[1];

        _levelTextLocalizeEvent.StringReference.Arguments[0] = _collected.GetLevel(_cardItem.CardID);
        _levelTextLocalizeEvent.RefreshString();

        _itemNameText.text = _cardItem.CardName.GetText(_localizationSelector.CurrentLanguage);

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

    protected virtual void UpdateLocaleText()
    {
        if(_cardItem != null)
            _itemNameText.text = _cardItem.CardName.GetText(_localizationSelector.CurrentLanguage);
    }
}
