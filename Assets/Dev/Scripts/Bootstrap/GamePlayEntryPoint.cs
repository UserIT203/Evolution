using System.Collections;
using UnityEngine;
using Zenject;

public class GamePlayEntryPoint : MonoBehaviour
{
    [Inject] private DiContainer _container;

    [SerializeField] private QuestModel _questModel;
    [SerializeField] private QuestPresentation _questPresentation;

    private IEnumerator Start()
    {
        _container.Resolve<LevelManager>().Initialized();

        _container.Inject(_container.Resolve<SaveManager>());

        _questModel.Initialized();
        _questPresentation.Initialized();
        _container.Resolve<SaveManager>().Initialized();
       
        _container.Resolve<MenuManager>().Initialized();
        _container.Resolve<AbilityManager>().Initialized();
        _container.Resolve<GlobalManager>().Initialized();
        _container.Resolve<LevelUpgrade>().Initialized();

        yield return _container.Resolve<LocalizationSelector>()
            .SetLocalization(_container.Resolve<LocalizationSelector>().CurrentLanguage);
    }
}
