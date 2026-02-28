using UnityEngine;
using Zenject;

public abstract class ShopItem
{
    [Inject] private GlobalManager _globalManager;

    public Sprite Icon;
    public int Price;
    public bool UseDonatMoney;

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

    protected abstract void Success();

    protected virtual void Fail()
    {
        Debug.LogWarning("No needed money");
    }
}
