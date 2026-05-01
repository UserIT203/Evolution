using UnityEngine;
using Zenject;

public class InputInstaller : MonoInstaller
{
    [Inject] private YandexSDK _yandexSDK;

    public override void InstallBindings()
    {
        if (_yandexSDK.IsDesktop)
            Container.Bind<DesktopInput>().FromNew().AsSingle();
        else
            Container.Bind<DesktopInput>().FromMethod(ctx => null).AsSingle();
    }
}
