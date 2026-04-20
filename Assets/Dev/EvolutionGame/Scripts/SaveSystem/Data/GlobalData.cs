using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    public int GemCount;
    public CardInfo[] CardCollection;
    public CardInfo[] AbilityCardCollection;
    public string ActiveAbilityID;

    public GlobalData()
    {
        GemCount = 0;
        CardCollection = new CardInfo[0];
        AbilityCardCollection = new CardInfo[0];
        ActiveAbilityID = string.Empty;
    }
}

[System.Serializable]
public class CardInfo
{
    public string ID;
    public int CollectedCardCount;
    public int CardLevel;

    public CardInfo(string id, int collectedCard, int level)
    {
        ID = id;
        CollectedCardCount = collectedCard;
        CardLevel = level;
    }
}