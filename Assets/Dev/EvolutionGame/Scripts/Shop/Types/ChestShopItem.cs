using UnityEngine;
using Zenject;

[System.Serializable]
public class ChestShopItem : ShopItem
{
    [Inject] private GlobalManager _globalManager;

    [SerializeField] private ChestConfig _chestConfig;

    public override void TryBuy()
    {
        if (_globalManager.TryRemoveCoin(Price))
            Success();
        else
            Fail();
    }

    protected override void Success()
    {
        _menuManager.GetUIMenu<ChestMenu>().OpenChest(_chestConfig);
    }
}
