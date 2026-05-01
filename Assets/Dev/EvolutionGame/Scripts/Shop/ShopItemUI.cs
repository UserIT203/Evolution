using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ShopItemUI : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Image _icon;

    private ShopItem _item;
    private LocalizationSelector _localizationSelector;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Initialized(ShopItem item, LocalizationSelector selector)
    {
        _localizationSelector = selector;
        _localizationSelector.onChangeLocale += UpdateLocaleText;

        Debug.Log(_localizationSelector);

        _icon.sprite = item.Icon;
        _priceText.text = item.Price.ToString();
        _nameText.text = item.NameItem.GetText(_localizationSelector.CurrentLanguage);

        if(item is GemShopItem gemItem)
        {
            gemItem.onShowReward += UpdatePriceText;
        }

        _item = item;

        _button.onClick.AddListener(() => item.TryBuy());
    }

    private void UpdateLocaleText()
    {
        Debug.Log("Update Text in Shop");
        _nameText.text = _item.NameItem.GetText(_localizationSelector.CurrentLanguage);
    }

    private void UpdatePriceText(int count)
    {
        Debug.Log($"Show Reward {count}");

        _priceText.text = $"{count} | {_item.Price}";
    }
}
   
