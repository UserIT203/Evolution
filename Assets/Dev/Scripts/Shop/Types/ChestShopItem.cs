using Zenject;

public class ChestShopItem : ShopItem
{
    private ChestConfig _chestConfig;

    public ChestShopItem(IPurchasedItem item)
    {
        Icon = item.CloseIcon;
        Price = item.Price;
        UseDonatMoney = item.UseDonatMoney;
        _chestConfig = item as ChestConfig;
        NameItem = _chestConfig.ToString();
    }

    protected override void Success()
    {
        _menuManager.GetUIMenu<ChestMenu>().OpenChest(_chestConfig);
    }
}
