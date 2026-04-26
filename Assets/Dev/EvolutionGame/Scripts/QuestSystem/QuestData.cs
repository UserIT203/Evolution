using UnityEngine;

[System.Serializable]
public class QuestData
{
    [field:SerializeField] public string QuestID {  get; private set; }
    [field:SerializeField] public QuestType Type { get; private set; }
    [field:SerializeField] public LocalizeText QuestName { get; private set; }
    [field:SerializeField] public int Goal { get; private set; }
    [field:SerializeField] public int Reward { get; private set; }

    public int Progress;
    public bool IsFinished;
    public bool GetReward;

    public QuestData(QuestScriptableObject so, int progress)
    {
        this.QuestName = so.QuestName;
        this.QuestID = so.QuestID;
        this.Type = so.Type;
        this.Goal = so.Goal;
        this.Reward = so.Reward;
  
        this.Progress = progress;
        this.IsFinished = false;
        this.GetReward = false;
    }
}
