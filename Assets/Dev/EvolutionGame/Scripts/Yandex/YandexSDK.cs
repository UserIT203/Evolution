using System;
using System.Collections.Generic;
using YG;
using UnityEngine;

public class YandexSDK
{
    private Dictionary<string, int> _rewardedDict = new();
    private Dictionary<string, (int, Action)> _rewardedActionDict = new();

    public Action onSuccessReward;

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
        if (_rewardedDict.TryGetValue(id, out var count) == true)
            return count;
            
        return 0;
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
}
