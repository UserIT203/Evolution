using UnityEngine;
using Zenject;

public abstract class ShopItem
{
    [Inject] private YandexSDK _yandexSDK;
    [Inject] private GlobalManager _globalManager;
    [Inject] protected MenuManager _menuManager;

    public Sprite Icon;
    public int Price;

    public LocalizeText NameItem;

    public abstract void TryBuy();

    protected virtual void Success()
    {

    }

    protected virtual void Fail()
    {
        Debug.LogWarning("<color=red>No needed money</color>");
    }
}
