using UnityEngine;
using System.Collections.Generic;
using Zenject;

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

    [Inject] private DiContainer _diContainer;

    [SerializeField] private List<Containers> _containers;

    [Header("Shop Item Info")]
    [SerializeField] private List<ChestConfig> _chests;
    [SerializeField] private List<CoinShopItem> _coins;

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

                shopUI.Initialized(item);
            }
        }
    }

    private void InitChestItems(Containers container)
    {
        container.Items = new List<ShopItem>();

        foreach (ChestConfig chest in _chests)
        {
            ChestShopItem shopItem = new ChestShopItem(chest);

            _diContainer.Inject(shopItem);

            container.Items.Add(shopItem);
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
}
