using UnityEngine;

public interface ISaveSystem
{
    public T LoadData<T>(string fileName = null) where T : new();
    public void SaveDate<T>(T saveData, string fileName = null) where T: SaveData;
}
