using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ShopItemUI : MonoBehaviour
{
    [Header("Sprite Type")]
    [SerializeField] private Sprite _donatCoin;
    [SerializeField] private Sprite _defaultCoin;

    [Header("UI Links")]
    [SerializeField] private Image _coinTypeImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;

    private Image _icon;
    private Button _button;

    private void Awake()
    {
        _icon = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Initialized(ShopItem item)
    {
        _icon.sprite = item.Icon;
        _priceText.text = item.Price.ToString();

        Sprite sprite = item.UseDonatMoney == true ? _donatCoin : _defaultCoin;
        _coinTypeImage.sprite = sprite;

        _button.onClick.AddListener(() => item.TryBuy());
    }
}
   
