using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Zenject;

public class GamePlayEntryPoint : Bootstrap
{
    [Inject] private ItemManager _itemManager;
    [Inject] private DiContainer _container;

    [SerializeField] private QuestModel _questModel;
    [SerializeField] private QuestPresentation _questPresentation;

    public override async UniTask Initialized(ISceneArgs args)
    {
        Debug.Log("Start Init Gameplay Bootstrap");

        _itemManager.Initiliazed(_container.Resolve<LevelUpgrade>());

        _container.Inject(_container.Resolve<SaveManager>());

        _questModel.Initialized();

        _container.Resolve<LevelManager>().Initialized();
        _container.Resolve<SaveManager>().Initialized();

        _questPresentation.Initialized();

        _container.Resolve<MenuManager>().Initialized();
        _container.Resolve<AbilityManager>().Initialized();
        _container.Resolve<GlobalManager>().Initialized();
        _container.Resolve<LevelUpgrade>().Initialized();

        await _container.Resolve<LocalizationSelector>()
            .SetLocalization(_container.Resolve<LocalizationSelector>().CurrentLanguage);

        Debug.Log("End Init Gameplay Bootstrap");
    }
}
