using System.Collections;
using UnityEngine;
using Zenject;

public class BootstarpEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneLoader _sceneLoader;

    private IEnumerator Start()
    {
        var container = ProjectContext.Instance.Container;
        var saveManager = container.Resolve<SaveManager>();

        saveManager.Initialized();

        LocalizationSelector localizeSelector = container.Resolve<LocalizationSelector>();

        yield return localizeSelector.SetLocalization(localizeSelector.CurrentLanguage);

        LoadScene(container.Resolve<PlayerData>());
    }

    private void LoadScene(PlayerData playerData)
    {
        if (playerData.IsNewUser == true)
            _sceneLoader.LoadScene(2);
        else
            _sceneLoader.LoadScene(1);
    }
}
