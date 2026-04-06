using Unity.VisualScripting;
using UnityEngine;

public interface ISaveSystemService
{
    public void LoadData();
    public void SaveData(SaveSystem saveSystem);
}
