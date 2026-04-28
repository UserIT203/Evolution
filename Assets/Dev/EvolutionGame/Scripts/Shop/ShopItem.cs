using UnityEngine;
using Zenject;

public abstract class ShopItem
{
    [Inject] private GlobalManager _globalManager;
    [Inject] protected MenuManager _menuManager;

    public Sprite Icon;
    public int Price;
    public bool UseDonatMoney;
    public LocalizeText NameItem;

    public void TryBuy()
    {
        if(UseDonatMoney == true)
        {
            Debug.Log("Try Buy");

            if (_globalManager.TryRemoveCoin(Price))
                Success();
            else
                Fail();
        }
        else
        {
            //Внутреигровая покупка
        }
    }

    protected virtual void Success()
    {

    }

    protected virtual void Fail()
    {
        Debug.LogWarning("<color=red>No needed money</color>");
    }
}
