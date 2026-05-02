using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SaveManager : MonoBehaviour, IInitialized
{
    private const float TIME_TO_SAVE = 60;

    [Inject] private List<ISaveSystemService> _services;
    [Inject] private ISaveSystem _saveSystem;

    private float _lastTimeSave;

    private void OnDisable()
    {
        SaveAllData();
    }

    private void Awake()
    {
        Application.quitting += SaveAllData;
    }

    private void Update()
    {
        if(Time.time - _lastTimeSave > TIME_TO_SAVE)
        {
            SaveAllData();
            _lastTimeSave = Time.time;
        }
    }

    public void Initialized()
    {
        _services.ForEach(s => s.LoadData());
        Debug.Log($"<color=green>Inject Services Count</color> {_services.Count}");
    }

    public void SaveAllData()
    {
        Debug.Log("<color=red>Save All Data</color>");
        _services.ForEach(s => s.SaveData(_saveSystem));
    }
}
