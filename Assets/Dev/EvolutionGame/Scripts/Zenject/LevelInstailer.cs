using UnityEngine;
using Zenject;

public class LevelInstailer : MonoInstaller
{
    public override void InstallBindings()
    {
        InjectBootableComponent();

        Container.Bind<EraManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WaveManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LevelUpgrade>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LootManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<AbilityManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UpgradesMenu>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<ISaveSystemService>().FromComponentsInHierarchy().AsCached();

        Debug.Log("[DEBUG IN WEB] Load Level Inject");
    }

    private void InjectBootableComponent()
    {
        Container.Bind<MenuManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GlobalManager>().FromComponentInHierarchy().AsSingle();
    }
}
