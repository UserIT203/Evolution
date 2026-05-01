using UnityEngine;
using Zenject;

[System.Serializable]
public class CoinShopItem : ShopItem
{
    [Inject] private GlobalManager _globalManager;
    [Inject] private LevelUpgrade _levelUpgrade;

    [SerializeField] private int _getCoins;

    public override void TryBuy()
    {
        if (_globalManager.TryRemoveCoin(Price))
            Success();
        else
            Fail();
    }

    protected override void Success()
    {
        _levelUpgrade.AddCoin(_getCoins);
    }
}
