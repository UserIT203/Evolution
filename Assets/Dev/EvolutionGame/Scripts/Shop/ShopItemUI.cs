using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ShopItemUI : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] protected TMP_Text _nameText;
    [SerializeField] protected TMP_Text _priceText;
    [SerializeField] private Image _icon;

    private ShopItem _item;
    protected LocalizationSelector _localizationSelector;
    protected Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public virtual void Initialized(ShopItem item, LocalizationSelector selector)
    {
        _localizationSelector = selector;
        _localizationSelector.onChangeLocale += UpdateLocaleText;

        Debug.Log(_localizationSelector);

        _icon.sprite = item.Icon;
        _priceText.text = item.Price.ToString();
        _nameText.text = item.NameItem.GetText(_localizationSelector.CurrentLanguage);

        _item = item;

        if (item is GemShopItem gemItem)
        {
            gemItem.onShowReward += UpdatePriceText;
            item.Initialized();
        }

        _button.onClick.AddListener(() => item.TryBuy());
    }

    protected void UpdateLocaleText()
    {
        Debug.Log("Update Text in Shop");
        _nameText.text = _item.NameItem.GetText(_localizationSelector.CurrentLanguage);
    }

    protected void UpdatePriceText(int count)
    {
        _priceText.text = $"{count} | {_item.Price}";
    }
}
   
