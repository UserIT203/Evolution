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

    public override void Initialized()
    {
        Debug.Log($"[SHOP MANAGER IN WEB] Gem Shop {_yandexSDK}");
        _yandexSDK.onSuccessReward += SuccessRewardShow;
        onShowReward?.Invoke(_yandexSDK.GetRewardInfo(_rewardID));
    }

    public override void TryBuy()
    {
        _yandexSDK.ShowRewardADV(id: _rewardID, completedAction: Success, advCount: Price);
    }

    protected override void Success()
    {
        _globalManager.GemCount = _getGems;
    }

    private void SuccessRewardShow()
    {
        onShowReward?.Invoke(_yandexSDK.GetRewardInfo(_rewardID));
    }
}
