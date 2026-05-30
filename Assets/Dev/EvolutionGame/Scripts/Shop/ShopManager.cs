using UnityEngine;
using System.Collections.Generic;
using Zenject;
using PaymentModels;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    private class Containers
    {
        public ShopItemType ContainerType;
        public Transform Container;
        public ShopItemUI Visualization;
        public List<ShopItem> Items;
    }

    [Inject] private YandexSDK _yandexSDK;
    [Inject] private LocalizationSelector _localizationSelector;
    [Inject] private DiContainer _diContainer;

    [SerializeField] private List<Containers> _containers;

    [Header("Shop Item Info")]
    [SerializeField] private List<ChestShopItem> _chests;
    [SerializeField] private List<CoinShopItem> _coins;
    [SerializeField] private List<GemShopItem> _gems;

    private void Awake()
    {
        InitializedShopItem();
    }
      
    private void InitializedShopItem()
    {
        foreach (var container in _containers)
        {
            switch (container.ContainerType)
            {
                case ShopItemType.Chest:
                    InitChestItems(container);
                    break;

                case ShopItemType.LevelMoney:
                    InitCoinItems(container);
                    break;

                case ShopItemType.DonatMoney:
                    InitGemItems(container);
                    break;

                case ShopItemType.GamePass:
                    InitKitItems(container);
                    break;
            }
        }

        InitializedUI();
    }

    private void InitializedUI()
    {
        foreach(var container in _containers)
        {
            foreach (var item in container.Items)
            {
                ShopItemUI shopUI = Instantiate(
                    container.Visualization,
                    container.Container) 
                    as ShopItemUI;

                _diContainer.Inject(shopUI);

                shopUI.Initialized(item, _localizationSelector);
            }
        }
    }

    private void InitChestItems(Containers container)
    {
        container.Items = new List<ShopItem>();

        foreach (ChestShopItem chest in _chests)
        {
            _diContainer.Inject(chest);
            container.Items.Add(chest);
        }
    }

    private void InitCoinItems(Containers container)
    {
        container.Items = new List<ShopItem>();

        foreach (var item in _coins)
        {
            _diContainer.Inject(item);  
            container.Items.Add(item);
        }
    }

    private void InitGemItems(Containers container)
    {
        container.Items = new List<ShopItem>();

        foreach (var item in _gems)
        {
            _diContainer.Inject(item);
            container.Items.Add(item);
        }
    }

    private void InitKitItems(Containers container)
    {
        container.Items = new List<ShopItem>();

        foreach (StartKit payment in _yandexSDK.PaymentsItem)
        {
            StartKitItem item = new StartKitItem(payment);
            _diContainer.Inject(item);
            item.Initialized();
            container.Items.Add(item);
        }
    }
}
