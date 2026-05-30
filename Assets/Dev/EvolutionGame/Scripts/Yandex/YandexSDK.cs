using System;
using System.Collections.Generic;
using YG;
using UnityEngine;
using PaymentModels;
using YG.Utils.Pay;
using System.Linq;

public class YandexSDK
{
    private Dictionary<string, int> _rewardedDict = new();
    private Dictionary<string, (int, Action)> _rewardedActionDict = new();

    private Dictionary<string, StartKit> _paymentItemDict = new();
    private Dictionary<string, Action> _paymentActionDict = new();

    public Action onSuccessReward;
    public Action onSuccessPayment;

    public StartKit[] PaymentsItem
    {
        get
        {
            return _paymentItemDict.Values.ToArray();
        }
    }

    public bool IsDesktop
    {
        get
        {
            return YG2.envir.isDesktop;
        }
    }
    public int Language
    {
        get
        {
            switch (YG2.envir.language)
            {
                case "en":
                    return 0;
                case "ru":
                    return 1;
                case "tr":
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public YandexSDK()
    {
        _rewardedDict = YG2.GetAllStats();

        YG2.onRewardAdv += HandleRewardedSuccess;
        YG2.onErrorRewardedAdv += HandleRewardedFailed;
        YG2.onCloseRewardedAdv += HandleRewardedClose;

        YG2.onPurchaseSuccess += SuccessPurchase;
        YG2.onPurchaseFailed += FailedPurchase;

        InitializedPaymentItem();
    }

    public void ShowInterstitialADV()
    {
        YG2.InterstitialAdvShow();
    }

    public void ShowRewardADV(string id, Action completedAction = null, int advCount = 0)
    {
        if (YG2.nowRewardAdv == true)
            return;

        if(advCount == 0)
        {
            YG2.RewardedAdvShow(id, completedAction);
            return;
        }
        
        if(_rewardedDict.ContainsKey(id) == true)
        {
            if(_rewardedActionDict.ContainsKey(id) == false)
                _rewardedActionDict.Add(id, (advCount, completedAction));
        }
        else
        {
            _rewardedDict.Add(id, 0);
        }

        YG2.RewardedAdvShow(id);
    }

    public int GetRewardInfo(string id)
    {
        try
        {
            if (_rewardedDict.TryGetValue(id, out var count) == true)
                return count;
        }
        catch (Exception ex)
        {
            Debug.Log($"[REWARD ADV INFO] Error {ex}");
            _rewardedDict = new();
            _rewardedDict.Add(id, 0);
        }
        
            
        return 0;
    }

    public void RegisterPaymentAction(string id, Action action)
    {
        if (_paymentActionDict.ContainsKey(id)) return;
        _paymentActionDict.Add(id, action);
    }

    public void UnregisterPaymentActio(string id)
    {
        if (_paymentActionDict.ContainsKey(id) == false) return;
        _paymentActionDict.Remove(id);
    }

    public StartKit GetPaymentReward(string id)
    {
        return _paymentItemDict[id];
    }

    private void InitializedPaymentItem()
    {
        Purchase[] purchases = YG2.purchases;

        foreach (Purchase purchase in purchases)
        {
            _paymentItemDict.Add(purchase.id, new StartKit(purchase.id));
            Debug.Log($"[PAYMENTS] Purchases ID {purchase.id}");
        }  
    }

    private void HandleRewardedSuccess(string rewardID)
    {
        Debug.Log("[REWARD] Success Reward Show");

        if (_rewardedDict.TryGetValue(rewardID, out int count))
        {
            count++;
            _rewardedDict[rewardID] = count;
        }

        if (_rewardedActionDict.ContainsKey(rewardID))
        {
            if(count >= _rewardedActionDict[rewardID].Item1)
            {
                _rewardedDict[rewardID] = 0;
                _rewardedActionDict[rewardID].Item2();
            }
        }

        onSuccessReward?.Invoke();
    }

    private void HandleRewardedFailed()
    {
        Debug.Log("[REWARD] Reward Failed");
    }

    private void HandleRewardedClose()
    {
        Debug.Log("[REWARD] Close Reward ADV");
    }

    private void SuccessPurchase(string id)
    {
        Debug.Log($"[INAP] Success payment with id {id}");

        YG2.SetState(id, 1);
        _paymentActionDict[id]?.Invoke();
        
        onSuccessPayment?.Invoke();    
    }

    private void FailedPurchase(string id)
    {
        Debug.Log($"[INAP] Failed payment with id {id}");
    }
}
