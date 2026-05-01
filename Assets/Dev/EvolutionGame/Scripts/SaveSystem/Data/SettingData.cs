using UnityEngine;

[System.Serializable]
public class SettingData : SaveData
{
    public float MusicVolume;
    public float SFXVolume;
    public int LocaleIndex;

    public SettingData()
    {
        MusicVolume = 0f;
        SFXVolume = 0f;
        LocaleIndex = 0;
    }
}