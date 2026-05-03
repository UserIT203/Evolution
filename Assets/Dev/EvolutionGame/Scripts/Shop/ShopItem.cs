using UnityEngine;
using Zenject;

public abstract class ShopItem
{
    [Inject] protected MenuManager _menuManager;

    public Sprite Icon;
    public int Price;

    public LocalizeText NameItem;

    public abstract void TryBuy();

    public virtual void Initialized()
    {

    }

    protected virtual void Success()
    {

    }

    protected virtual void Fail()
    {
        Debug.LogWarning("<color=red>No needed money</color>");
    }
}
