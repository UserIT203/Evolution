using UnityEngine;
using Zenject;

public class InputInstaller : MonoInstaller
{
    [SerializeField] private bool _isDesktop;

    public override void InstallBindings()
    {
        if (_isDesktop == true)
            Container.Bind<DesktopInput>().FromNew().AsSingle();
        else
            Container.Bind<DesktopInput>().FromMethod(ctx => null).AsSingle();
    }
}
