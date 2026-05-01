using UnityEngine;
using Zenject;

public class GlobalIntstaller : MonoInstaller
{
    [SerializeField] private bool _isYandexBuild;
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private AudioManager _audioManager;

    private ISaveSystem _saveSystem;

    public override void InstallBindings()
    {
        LoadSaveData();
        LoadMain();
    }

    private void LoadSaveData()
    {
        if (_isYandexBuild == true)
            _saveSystem = new YandexSave();
        else
            _saveSystem = new SaveSystem();

        Container.BindInstance(_saveSystem).AsSingle().NonLazy();

        SettingData settingData = _saveSystem.LoadData<SettingData>("SettingData");
        Container.BindInstance(settingData).AsSingle().NonLazy();

        GlobalData globalData = _saveSystem.LoadData<GlobalData>("GlobalData");
        Container.BindInstance(globalData).AsSingle().NonLazy();

        LevelData levelData = _saveSystem.LoadData<LevelData>("LevelData");
        Container.BindInstance(levelData).AsSingle().NonLazy();

        PlayerData playerData = _saveSystem.LoadData<PlayerData>("PlayerData");
        Container.BindInstance(playerData).AsSingle().NonLazy();

        QuestSaveData questData = _saveSystem.LoadData<QuestSaveData>("QuestSaveData");
        Container.BindInstance(questData).AsSingle().NonLazy();
    }

    private void LoadMain()
    {
        Container.BindInterfacesAndSelfTo<YandexSDK>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<LocalizationSelector>().AsSingle().NonLazy();

        AudioManager audioManager = Container.InstantiatePrefabForComponent<AudioManager>
            (
            _audioManager, 
            Vector3.zero, 
            Quaternion.identity, 
            null
            );
        Container.BindInterfacesAndSelfTo<AudioManager>().FromInstance(audioManager);

        SaveManager saveManager = Container.InstantiatePrefabForComponent<SaveManager>
            (
            _saveManager, 
            Vector3.zero, 
            Quaternion.identity, 
            null
            );
        Container.BindInstance(saveManager).AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<ItemManager>().AsSingle().NonLazy();

        Debug.Log("<color=red>Create SaveManager</color>");
    }
}
