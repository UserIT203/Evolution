using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Zenject;

public class LocalizationSelector : ISaveSystemService
{
    [Inject] private SettingData _settingData;

    public int CurrentLanguage { get; private set; }

    private bool _isActive = false;

    public Action onChangeLocale;

    public IEnumerator SetLocalization(int localeID)
    {
        if(_isActive == true) yield return null;

        _isActive = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];

        CurrentLanguage = localeID;

        _isActive = false;

        Debug.Log($"<color=yellow>Change Language</color> {localeID}");

        onChangeLocale?.Invoke();
    }

    public void LoadData()
    {
        
    }

    public void SaveData(SaveSystem saveSystem)
    {
        Debug.Log("<color=red>Save Locale</color>");

        _settingData.LocaleIndex = CurrentLanguage;
        saveSystem.SaveDate(_settingData, "SettingData");
    }
}
