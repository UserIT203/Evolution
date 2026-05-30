using TMPro;
using UnityEngine;
using YG;
using YG.Utils.Pay;

public class KitItemUI : ShopItemUI
{
    [SerializeField] private TMP_Text _coinCount;
    [SerializeField] private TMP_Text _gemCount;

    [SerializeField] private LocalizeText _buyedText;

    private StartKitItem _kitItem;
    private Purchase _data;

    private void OnDestroy()
    {
        _kitItem.onBuyed -= UpdatePriceText;
    }

    public override void Initialized(ShopItem item, LocalizationSelector selector)
    {
        _localizationSelector = selector;
        _localizationSelector.onChangeLocale += UpdateTextFromLanguage;

        if (item is StartKitItem kitItem)
        {
            _data = YG2.PurchaseByID(kitItem.KitItem.ID);
            _kitItem = kitItem;

            _coinCount.text = kitItem.KitItem.CoinCount.ToString();
            _gemCount.text = kitItem.KitItem.GemCount.ToString();

            _nameText.text = item.NameItem.GetText(_localizationSelector.CurrentLanguage);

            UpdatePriceText();

            kitItem.onBuyed += UpdatePriceText;
        }

        _button.onClick.AddListener(() => item.TryBuy());
    }

    private void UpdateTextFromLanguage()
    {
        UpdatePriceText();

        _nameText.text = _kitItem.NameItem.GetText(_localizationSelector.CurrentLanguage);
    }

    private void UpdatePriceText()
    {
        if (_kitItem.KitItem.IsBuyed)
            _priceText.text = _buyedText.GetText(_localizationSelector.CurrentLanguage);
        else
            _priceText.text = string.Format($"{_data.price}");
    }
}
