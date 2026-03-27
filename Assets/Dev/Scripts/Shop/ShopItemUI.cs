using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ShopItemUI : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Image _icon;

    private Button _button;

    private void Awake()
    {
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
        _nameText.text = item.NameItem;

        _button.onClick.AddListener(() => item.TryBuy());
    }
}
   
