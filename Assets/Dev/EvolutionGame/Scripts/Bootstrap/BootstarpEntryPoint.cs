using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Zenject;

public class BootstarpEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneLoaderView _sceneLoaderView;

    private async UniTask Start()
    {
        SceneLoaderView sceneView = Instantiate(_sceneLoaderView);

        var container = ProjectContext.Instance.Container;

        var saveManager = container.Resolve<SaveManager>();

        saveManager.Initialized();

        container.Inject(sceneView);

        LocalizationSelector localizeSelector = container.Resolve<LocalizationSelector>();

        await localizeSelector.SetLocalization(container.Resolve<YandexSDK>().Language);

        await container.Resolve<AudioManager>().Initialized();

        LoadScene(container.Resolve<PlayerData>(), container.Resolve<SceneLoader>());
    }

    private void LoadScene(PlayerData playerData, SceneLoader sceneLoader)
    {
        if (playerData.IsNewUser == true)
            sceneLoader.SwitchScene("TutotialScene").Forget();
        else
            sceneLoader.SwitchScene("UI_Manager_Scene").Forget();
    }
}
