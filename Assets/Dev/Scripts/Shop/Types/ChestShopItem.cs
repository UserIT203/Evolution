using UnityEngine;

[System.Serializable]
public class ChestShopItem : ShopItem
{
    [SerializeField] private ChestConfig _chestConfig;

    protected override void Success()
    {
        _menuManager.GetUIMenu<ChestMenu>().OpenChest(_chestConfig);
    }
}
