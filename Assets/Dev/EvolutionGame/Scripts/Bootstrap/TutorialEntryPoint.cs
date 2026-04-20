using System.Collections;
using UnityEngine;
using Zenject;

public class TutorialEntryPoint : MonoBehaviour
{
    [Inject] private LevelManager _levelManager;
    [Inject] private GlobalManager _globalManager;
    [Inject] private LevelUpgrade _levelUpgrade;
    [Inject] private MenuManager _menuManager;
    [Inject] private LocalizationSelector _localizationSelector;


    private IEnumerator Start()
    {
        _levelManager.Initialized();
        _globalManager.Initialized();
        _levelUpgrade.Initialized();

        _menuManager.Initialized();

        yield return _localizationSelector
            .SetLocalization(_localizationSelector.CurrentLanguage);
    }
}
