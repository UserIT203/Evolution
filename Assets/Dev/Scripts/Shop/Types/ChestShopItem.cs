using UnityEngine;
using Zenject;

public class ChestShopItem : ShopItem
{
    [Inject] private ChestManager _chestManager;

    private Rarity _chestRarity;

    public string NameItem => _chestRarity.ToString();

    public ChestShopItem(IPurchasedItem item, Rarity chestRarity)
    {
        Icon = item.Icon;
        Price = item.Price;
        UseDonatMoney = item.UseDonatMoney;
        _chestRarity = chestRarity;
    }

    protected override void Success()
    {
        _chestManager.OpenChest(_chestRarity);
    }
}
