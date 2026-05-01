using UnityEngine;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public List<LoadData> Data = new();

        public string GetJsonData(System.Type type)
        {
            LoadData data = Data.Find(i => i.Type == type);

            return data?.JsonData;
        }

        public void SetData<T>(T saveData) where T : SaveData
        {
            System.Type type = typeof(T);
            LoadData loadData = Data.Find(i => i.Type == type);

            Debug.Log($"Data Save {loadData == null}");

            if (loadData == null)
                Data.Add(new LoadData(type, saveData));
            else
                loadData.JsonData = UnityEngine.JsonUtility.ToJson(saveData);
        }
    }
}

[System.Serializable]
public class LoadData
{
    public System.Type Type;
    public string JsonData;

    public LoadData(System.Type type, SaveData obj)
    {
        Type = type;
        JsonData = UnityEngine.JsonUtility.ToJson(obj);
    }
}
