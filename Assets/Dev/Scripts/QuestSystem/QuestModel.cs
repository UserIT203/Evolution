using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System;

public class QuestModel : MonoBehaviour, IInitialized
{
    [Inject] private GlobalManager _globalManager;

    [Header("<color=green>Main Settings</color>")]
    [SerializeField] private QuestScriptableObject[] _questList;
    public TimeSpan TimeToUpdateQuests = new TimeSpan(0, 0, 0);

    public Dictionary<QuestType, List<QuestData>> TypeQuestDictionary;
    public List<QuestData> ActiveQuest { get; set; }
    public List<QuestData> Data { get; set; }
    
    public DateTime? LastUpdateTime
    {
        get
        {
            string data = PlayerPrefs.GetString("lastClaimTime", null);

            if(string.IsNullOrEmpty(data) == false)
                return DateTime.Parse(data);

            return null;
        }

        set
        {
            if (value != null)
                PlayerPrefs.SetString("lastClaimTime", value.ToString());
            else
                PlayerPrefs.DeleteKey("lastClaimTime");
        }
    }

    public void Initialized()
    {
        Data = new List<QuestData>();
        TypeQuestDictionary = new();
        ActiveQuest = new List<QuestData>();

        foreach (var quest in _questList)
        {
            if (TypeQuestDictionary.ContainsKey(quest.Type))
                TypeQuestDictionary[quest.Type].Add(new QuestData(quest, 0));
            else
                TypeQuestDictionary.Add(quest.Type, new List<QuestData> { new QuestData(quest, 0) });

            Data.Add(new QuestData(quest, 0));
        }
    }

    public void OnFinish(QuestData data)
    {
        data.IsFinished = true;
        _globalManager.GemCount = data.Reward;
    }

    public QuestData GetQuestDataFromID(string id) => Data.Find(i => i.QuestID == id);

    public List<QuestData> GetQuestDatasFromType(QuestType type) => TypeQuestDictionary[type];
}