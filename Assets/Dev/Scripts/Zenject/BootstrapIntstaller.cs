using UnityEngine;
using Zenject;

public class BootstrapIntstaller : MonoInstaller
{
    private SaveSystem _saveSystem;

    public override void InstallBindings()
    {
        LoadSaveData();
    }

    private void LoadSaveData()
    {
        _saveSystem = new SaveSystem();

        SettingData settingData = _saveSystem.LoadData<SettingData>("SettingData");
        Container.BindInstance(settingData).AsSingle().NonLazy();

        GlobalData globalData = _saveSystem.LoadData<GlobalData>("GlobalData");
        Container.BindInstance(globalData).AsSingle().NonLazy();

        LevelData levelData = _saveSystem.LoadData<LevelData>("LevelData");
        Container.BindInstance(levelData).AsSingle().NonLazy();
    }
}
