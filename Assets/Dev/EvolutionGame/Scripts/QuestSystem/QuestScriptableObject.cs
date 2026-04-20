using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Quest")]
public class QuestScriptableObject : ScriptableObject
{
    public string QuestID;
    [Space(5f)]
    public QuestType Type;
    public LocalizeText QuestName;
    public int Goal;
    public int Reward;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(QuestID) == true)
            QuestID = GetHashCode().ToString();
    }
}
