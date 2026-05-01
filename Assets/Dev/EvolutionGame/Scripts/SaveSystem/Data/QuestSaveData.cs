public class QuestSaveData : SaveData
{
    public QuestInfo[] ActiveQuestData;
}

[System.Serializable]
public class QuestInfo
{
    public string ID;
    public int Progress;
    public bool GetReward;
}