using System;
using System.Collections.Generic;
using YG;
using UnityEngine;

public class YandexSDK
{
    private Dictionary<string, int> _rewarded;

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
        _rewarded = new();
    }

    public void ShowInterstitialADV()
    {
        YG2.InterstitialAdvShow();
    }

    public void ShowRewardADV(string id, Action completedAction = null, int advCount = 0)
    {
        if(advCount == 0)
        {
            YG2.RewardedAdvShow(id, completedAction);
            return;
        }
            
        if (_rewarded.TryGetValue(id, out var count) == true)
        {
            count++;
            _rewarded[id] = count;

            if(_rewarded[id] >= advCount)
            {
                YG2.RewardedAdvShow(id, completedAction);
                _rewarded.Remove(id);
            }              
        }
        else
            _rewarded.Add(id, 1);

        YG2.RewardedAdvShow(id);
    }

    public int GetRewardInfo(string id)
    {
        if (_rewarded.TryGetValue(id, out var count) == true)
            return count;
            
        return 0;
    }
}
