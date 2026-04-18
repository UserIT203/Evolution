using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelInstailer : MonoInstaller
{
    public override void InstallBindings()
    {
        InjectBootableComponent();

        Container.Bind<EraManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LevelBuilder>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UnitSpawner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WaveManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LevelUpgrade>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ItemManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ChestManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LootManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<AbilityManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UpgradesMenu>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<ISaveSystemService>().FromComponentsInHierarchy().AsCached();
    }

    private void InjectBootableComponent()
    {
        Container.Bind<MenuManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GlobalManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<QuestPresentation>().FromComponentInHierarchy().AsSingle();
    }
}
