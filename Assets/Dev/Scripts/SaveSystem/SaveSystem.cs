using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveSystem
{
    public string GetFullPath(string fileName)
    {
        return Application.dataPath + Path.AltDirectorySeparatorChar + fileName + ".json";
    }

    public void SaveDate<T>(T saveData, string fileName = null)
    {
        string json = JsonUtility.ToJson(saveData);
        
        fileName = string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName;

        File.WriteAllText(GetFullPath(fileName), json);
    }

    public T LoadData<T>(string fileName) where T : new()
    {
        string json = string.Empty;

        if (File.Exists(GetFullPath(fileName)) == true)
        {
            json = File.ReadAllText(GetFullPath(fileName));
            T data = JsonUtility.FromJson<T>(json);

            return data;
        }

        return new();
    }
}
