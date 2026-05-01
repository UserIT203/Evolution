using System;
using UnityEngine;
using YG;

public class YandexSave : ISaveSystem
{
    public T LoadData<T>(string fileName = null) where T : new()
    {
        string jsonData = YG2.saves.GetJsonData(typeof(T));

        if(string.IsNullOrEmpty(jsonData) == false)
        {
            try
            {
                return UnityEngine.JsonUtility.FromJson<T>(jsonData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LoadData] Ошибка десериализации {typeof(T).Name}: {e.Message}");
            }
        }

        Debug.Log($"[LoadDATA] Data don't found type of {typeof(T).Name}.Defaul Data");

        return new T();
    }

    public void SaveDate<T>(T saveData, string fileName = null) where T: SaveData
    {
        Debug.Log("SAVE DATA IN YANDEX</color>");

        YG2.saves.SetData(saveData);
        YG2.SaveProgress();
    }
}
