using Cysharp.Threading.Tasks;
using Zenject;
using UnityEngine;

public class TutorialEntryPoint : Bootstrap
{
    [Inject] private LevelManager _levelManager;
    [Inject] private GlobalManager _globalManager;
    [Inject] private LevelUpgrade _levelUpgrade;
    [Inject] private MenuManager _menuManager;
    [Inject] private LocalizationSelector _localizationSelector;

    [SerializeField] private TutorialManager _tutorialManager;

    public override async UniTask Initialized(ISceneArgs args)
    {
        Debug.Log("[INIT] Tutorial Scene");

        _levelManager.Initialized();
        _globalManager.Initialized();
        _levelUpgrade.Initialized();

        _menuManager.Initialized();

        await _localizationSelector.SetLocalization(_localizationSelector.CurrentLanguage);

        _tutorialManager.Initialized();
    }
}
