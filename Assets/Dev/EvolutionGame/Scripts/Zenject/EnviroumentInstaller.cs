using UnityEngine;
using Zenject;

public class EnviroumentInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle().NonLazy();
    }
}