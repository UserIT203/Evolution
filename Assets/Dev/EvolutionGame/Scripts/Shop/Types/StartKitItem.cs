using PaymentModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using YG;
using Zenject;

[System.Serializable]
public class StartKitItem : ShopItem
{
    [Inject] private YandexSDK _yandex;
    [Inject] private GlobalManager _globalManager; 

    public StartKit KitItem { get; private set; }

    public Action onBuyed;

    public StartKitItem(StartKit payment)
    {
        KitItem = payment;

        List<Localize> locales = new()
        {
            new Localize(){LocalizeLabel = 0, Text = "Starter set"},
             new Localize(){LocalizeLabel = 1, Text = "Стартовый набор"},
             new Localize(){LocalizeLabel = 2, Text = "Başlangıç ​​seti"},
        };

        NameItem = new LocalizeText(locales.ToArray());

        Debug.Log($"[SHOP MANAGER IN WEB] Kit Item Shop");
    }

    public override void Initialized()
    {
        _yandex.RegisterPaymentAction(KitItem.ID, Success);

        Debug.Log($"[KIT ITEM] Is Buyed {KitItem}");
    }

    public override void TryBuy()
    {
        if (KitItem.IsBuyed == true) return;

        YG2.BuyPayments(KitItem.ID);
    }

    protected override void Success()
    {
        _globalManager.GemCount = KitItem.GemCount;
        Debug.Log($"[KIT ITEM] Buy {KitItem.ID}");

        onBuyed?.Invoke();
    }
}
