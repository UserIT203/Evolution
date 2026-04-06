using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SaveManager : MonoBehaviour, IInitialized
{
    [Inject] private List<ISaveSystemService> _services;

    private SaveSystem _saveSystem = new();

    private void OnDisable()
    {
        Debug.Log("<color=red>Save All Data</color>");
        _services.ForEach(s => s.SaveData(_saveSystem));
    }

    public void Initialized()
    {
        _services.ForEach(s => s.LoadData());
        Debug.Log($"<color=green>Inject Services Count</color> {_services.Count}");
    }
}
