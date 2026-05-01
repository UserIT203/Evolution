using UnityEngine;
using Zenject;
using System;

[System.Serializable]
public class GemShopItem : ShopItem
{
    [Inject] private YandexSDK _yandexSDK;
    [Inject] private GlobalManager _globalManager;

    [SerializeField] private string _rewardID;
    [SerializeField] private int _getGems;

    public Action<int> onShowReward;

    public override void TryBuy()
    {
        _yandexSDK.ShowRewardADV(id: _rewardID, completedAction: Success, advCount: Price);
        onShowReward?.Invoke(_yandexSDK.GetRewardInfo(_rewardID));
    }

    protected override void Success()
    {
        _globalManager.GemCount = _getGems;
    }
}
