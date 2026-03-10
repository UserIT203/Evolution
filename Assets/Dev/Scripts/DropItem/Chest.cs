using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class Chest : MonoBehaviour
{
    [Inject] private LootManager _lootManager;

    [SerializeField] private ChestConfig _chestConfig;

    public ChestConfig ChestConfig => _chestConfig;

    public void OpenChest()
    {
        if (_chestConfig == null || _lootManager == null) return;

        int itemCount = Random.Range(_chestConfig.MinItems, _chestConfig.MaxItems + 1);

        for (int i = 0; i < itemCount; i++)
        {
            CardItem loot = RollLootFromChest();

            if(loot != null)
            {
                _lootManager.LootHandler(loot);
            }
        }
    }

    private CardItem RollLootFromChest()
    {
        Rarity choisenRarity = RollRarityFromChest();

        var availableCards = _lootManager.DroppedLoots.GetLootsByRarity(choisenRarity);

        if (availableCards.Count == 0)
        {
            Debug.Log("No loot found for rarity");
            return null;
        }

        int index = Random.Range(0, availableCards.Count);

        return availableCards[index];
    }

    private Rarity RollRarityFromChest()
    {
        var weights = _chestConfig.RarityRollWeigth;
  
        if(weights == null || weights.Count == 0) return Rarity.Common;

        float roll = Random.Range(0f, 100f);
        Rarity droppedRarity = Rarity.Common;

        foreach (var weigth in weights)
        {
            if(roll <= weigth.Weigth)
                droppedRarity = weigth.Rarity;
        }

        return droppedRarity;
    }
}
