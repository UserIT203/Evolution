using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Zenject;

public class QuestPresentation : MonoBehaviour, IInitialized, ISaveSystemService
{
    [Inject] private QuestSaveData _saveData;

    [SerializeField] private float _timeToUpdateTimer;
    [SerializeField] private int _maxQuestCount = 4;
    [SerializeField] private QuestModel _model;
    [SerializeField] private QuestMenu _questMenu;

    private bool _isInitialized = false;
    private float _updateTimer;

    private void OnEnable()
    {
        QuestBus.GetInstance().onUpdateCounter += UpdateInfo;
    }

    private void OnDisable()
    {
        QuestBus.GetInstance().onUpdateCounter -= UpdateInfo;
    }

    private void Update()
    {
        if (_isInitialized == false) return;

        if(Time.time - _updateTimer > _timeToUpdateTimer)
        {
            CheackUpdateQuests();
            _updateTimer = Time.time;
        }
    }

    public void Initialized()
    {
        CheackUpdateQuests();
        _isInitialized = true;
    }

    public void UpdateInfo(QuestType type, int value)
    {
        List<QuestData> quests = _model.ActiveQuest.FindAll(i => i.Type == type);

        if(quests.Count <= 0)
        {
            Debug.LogWarning("Not Found Active Quests");
            return;
        }

        foreach (QuestData quest in quests)
        {
            quest.Progress += value;
            
            if(quest.Progress >= quest.Goal)
            {
                FinishQuest(quest.QuestID);
            }
        }

        QuestBus.GetInstance().onUpdateData?.Invoke();
    }


    private void FinishQuest(string id)
    {
        QuestData quest = _model.GetQuestDataFromID(id);

        if (quest == null) return;

        _questMenu.FinishQuest(quest);
        _model.OnFinish(quest);
    }

    private void CheackUpdateQuests()
    {
        if (_model.LastUpdateTime == null)
        {
            Debug.Log("<color=red>Don't Found Time</color>");
            _model.LastUpdateTime = DateTime.UtcNow;
            InitializedQuests();
            return;
        }

        if (DateTime.UtcNow.Month >= _model.LastUpdateTime.Value.Month
            && DateTime.UtcNow.Date > _model.LastUpdateTime.Value.Date
            && DateTime.UtcNow.Hour >= _model.TimeToUpdateQuests.Hours)
        {
            Debug.Log("<color=red>Update quests</color>");
            _model.LastUpdateTime = DateTime.UtcNow;
            InitializedQuests();
        }

        var todayReset = DateTime.UtcNow.Date.Add(_model.TimeToUpdateQuests);
        var nextUpdateTime = DateTime.UtcNow >= todayReset
            ? todayReset.AddDays(1) : todayReset;
        var timeRemain = nextUpdateTime - DateTime.UtcNow;

        _questMenu.UpdateTimer(timeRemain);
    }

    private void InitializedQuests()
    {
        _questMenu.UpdateInfo();

        if(_model.ActiveQuest.Count > 0)
            _model.ActiveQuest.ForEach(q => 
            {
                q.IsFinished = false;
                q.Progress = 0;
                q.GetReward = false; 
            });

        _model.ActiveQuest.Clear();

        HashSet<int> rollQuestType = new();

        while (rollQuestType.Count < _maxQuestCount)
            rollQuestType.Add(UnityEngine.Random.Range(0, Enum.GetValues(typeof(QuestType)).Length));

        int[] resultQuestType = rollQuestType.ToArray();

        int randomQuestIndex;
        int addedQuestCount = 0;

        while (addedQuestCount < _maxQuestCount)
        {
            List<QuestData> quests = _model.GetQuestDatasFromType((QuestType)resultQuestType[addedQuestCount]);
            randomQuestIndex = UnityEngine.Random.Range(0, quests.Count);

            if (CanStartQuest(quests[randomQuestIndex].QuestID) == true)
                addedQuestCount++;
        }
    }

    private bool CanStartQuest(string id, int progress = 0, bool getReward = false)
    {
        QuestData quest = _model.GetQuestDataFromID(id);

        if (_model.ActiveQuest.Contains(quest) == true)
        {
            Debug.LogWarning("Quest Already Added In List");
            return false;
        }

        if (quest == null)
        {
            Debug.LogWarning("Quest Don't Found");
            return false;
        }

        quest.Progress = progress;
        quest.GetReward = getReward;
        _model.ActiveQuest.Add(quest);
        _questMenu.LoadData(quest);

        return true;
    }

    public void LoadData()
    {
        if (_saveData.ActiveQuestData == null || _saveData.ActiveQuestData.Length <= 0) return;

        if (_saveData.ActiveQuestData.Length > 0)
        {
            foreach (var quest in _saveData.ActiveQuestData)
                CanStartQuest(quest.ID, quest.Progress, quest.GetReward);

            _model.LastUpdateTime = DateTime.UtcNow;
        }

        Debug.Log("Load Data In Quest");
    }

    public void SaveData(SaveSystem saveSystem)
    {
        _saveData.ActiveQuestData = new QuestInfo[_model.ActiveQuest.Count];

        for (int i = 0; i < _model.ActiveQuest.Count; i++)
        {
            _saveData.ActiveQuestData[i] = new QuestInfo()
            {
                ID = _model.ActiveQuest[i].QuestID,
                Progress = _model.ActiveQuest[i].Progress,
                GetReward = _model.ActiveQuest[i].GetReward
            };
        }

        saveSystem.SaveDate(_saveData, "QuestSaveData");
    }
}
