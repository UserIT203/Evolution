using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelInstailer : MonoInstaller
{
    public override void InstallBindings()
    {
        InjectGlobalComponent();

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
    }

    private void InjectGlobalComponent()
    {
        Container.Bind<GlobalManager>().FromComponentInHierarchy().AsSingle();
    }
}
