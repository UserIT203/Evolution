using UnityEngine;
using Zenject;

[System.Serializable]
public class CoinShopItem : ShopItem
{
    [Inject] private LevelUpgrade _levelUpgrade;

    [SerializeField] private int _getCoins;

    protected override void Success()
    {
        _levelUpgrade.PickUp(_getCoins);
    }
}
