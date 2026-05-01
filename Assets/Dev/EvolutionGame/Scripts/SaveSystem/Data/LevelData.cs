using UnityEngine;

public class LevelData : SaveData
{
    public int Coins;
    public bool[] OpenLevels;
    public int LevelUpgradeCount;
    public int CurrentOpenLevel;
    public bool[] UnlockUnits;

    public LevelData()
    {
        Coins = 0;
        LevelUpgradeCount = 0;
        CurrentOpenLevel = 0;
        OpenLevels = new bool[4];
        UnlockUnits = new bool[3];
    }
}
