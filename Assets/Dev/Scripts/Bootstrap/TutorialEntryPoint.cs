using UnityEngine;
using Zenject;

public class TutorialEntryPoint : MonoBehaviour
{
    [Inject] private DiContainer _container;


    private void Start()
    {
        _container.Resolve<LevelManager>().Initialized();
        _container.Resolve<GlobalManager>().Initialized();
        _container.Resolve<LevelUpgrade>().Initialized();

        _container.Resolve<MenuManager>().Initialized();
    }
}
