using UnityEngine;
using Zenject;

public class GamePlayEntryPoint : MonoBehaviour
{
    [Inject] private DiContainer _container;

    private void Start()
    {
        _container.Resolve<LevelManager>().Initialized();

        _container.Resolve<SaveManager>().Initialized();
        _container.Resolve<MenuManager>().Initialized();

        _container.Resolve<AbilityManager>().Initialized();
        _container.Resolve<GlobalManager>().Initialized();
        _container.Resolve<LevelUpgrade>().Initialized();

        LocalizationSelector selector = _container.Resolve<LocalizationSelector>();

        StartCoroutine(selector.SetLocalization(selector.CurrentLanguage));
    }
}
