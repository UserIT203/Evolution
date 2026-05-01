using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Zenject;

public class LocalizationSelector : ISaveSystemService
{
    [Inject] private SettingData _settingData;
    [Inject] private YandexSDK _yandexSDK;

    public int CurrentLanguage { get; private set; }

    private bool _isActive = false;

    public Action onChangeLocale;


    public async UniTask SetLocalization(int localeID)
    {
        if(_isActive == true) return;

        _isActive = true;

        await LocalizationSettings.InitializationOperation;
        
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];

        CurrentLanguage = localeID;

        _isActive = false;

        Debug.Log($"<color=yellow>Change Language</color> {localeID}");

        onChangeLocale?.Invoke();
    }

    public void LoadData()
    {
        CurrentLanguage = _settingData.LocaleIndex;
    }

    public void SaveData(ISaveSystem saveSystem)
    {
        Debug.Log("<color=red>Save Locale</color>");

        _settingData.LocaleIndex = CurrentLanguage;
        saveSystem.SaveDate(_settingData, "SettingData");
    }
}
