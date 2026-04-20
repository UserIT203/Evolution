using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Zenject;

public class BootstarpEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneLoaderView _sceneLoaderView;

    private IEnumerator Start()
    {
        SceneLoaderView sceneView = Instantiate(_sceneLoaderView);

        var container = ProjectContext.Instance.Container;

        var saveManager = container.Resolve<SaveManager>();

        saveManager.Initialized();

        container.Inject(sceneView);

        LocalizationSelector localizeSelector = container.Resolve<LocalizationSelector>();

        yield return localizeSelector.SetLocalization(localizeSelector.CurrentLanguage);

        LoadScene(container.Resolve<PlayerData>(), container.Resolve<SceneLoader>());
    }

    private void LoadScene(PlayerData playerData, SceneLoader sceneLoader)
    {
        if (playerData.IsNewUser == true)
            sceneLoader.SwitchScene(1).Forget();
        else
            sceneLoader.SwitchScene(1).Forget();
    }
}
