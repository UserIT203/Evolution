using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System;
using TMPro;

public class QuestMenu : Menu
{
    [Inject] private DiContainer _container;

    [Header("<color=yellow>Own Settings</color>")]
    [SerializeField] private Transform _questCellContainer;
    [SerializeField] private QuestCell _questCellPrefab;
    [SerializeField] private TMP_Text _timerText;

    private List<QuestData> _questData = new();
    private List<QuestCell> _questCell = new();

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();
    }

    public void FinishQuest(QuestData data)
    {
        QuestCell cell = _questCell.Find(i => i.Data == data);

        if (cell == null) return;
        
        cell.FinishQuest();
    }

    public void LoadData(QuestData quest)
    {
        _questData.Add(quest);
        CreateCell(quest);
    }

    public void UpdateTimer(TimeSpan time)
    {
        var timeTemp = time > TimeSpan.Zero ? time : TimeSpan.Zero;

        _timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
            (int)time.TotalHours,
            time.Minutes,
            time.Seconds);
    }

    public void UpdateInfo()
    {
        _questData.Clear();

        if (_questCell.Count > 0)
        {
            foreach (var cell in _questCell)
                Destroy(cell.gameObject);
        }

        _questCell.Clear();  
    }

    private void CreateCell(QuestData data)
    {
        QuestCell cell = Instantiate(_questCellPrefab);
        cell.transform.SetParent(_questCellContainer, false);

        _container.Inject(cell);

        cell.Init(data);

        _questCell.Add(cell);
    }
}
