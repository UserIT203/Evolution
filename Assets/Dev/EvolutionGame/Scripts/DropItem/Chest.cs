using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class Chest
{
    private AllLootPool _allLoots;

    public Chest(AllLootPool allLoot)
    {
        _allLoots = allLoot;
    }

    public CardItem[] GetDroppedCards(ChestConfig chestConfig)
    {
        int itemCount = Random.Range(chestConfig.MinItems, chestConfig.MaxItems + 1);
        CardItem[] droppedCards = new CardItem[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            CardItem loot = RollLootFromChest(chestConfig);

            if(loot != null)
            {
                droppedCards[i] = loot;
            }
        }

        return droppedCards;
    }

    private CardItem RollLootFromChest(ChestConfig config)
    {
        Rarity choisenRarity = RollRarityFromChest(config);

        var availableCards = _allLoots.GetLootsByRarity(choisenRarity);

        if (availableCards.Count == 0)
        {
            Debug.Log("No loot found for rarity");
            return null;
        }

        int index = Random.Range(0, availableCards.Count);

        return availableCards[index];
    }

    private Rarity RollRarityFromChest(ChestConfig config)
    {
        var weights = config.RarityRollWeigth;
  
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
