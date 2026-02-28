using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllLootPool", menuName = "Chest/AllLootPool")]
public class AllLootPool : ScriptableObject
{
    [SerializeField] private List<CardItem> DropedLoots = new();

    private Dictionary<Rarity, List<CardItem>> _lootsByRarity;

    public IReadOnlyList<CardItem> GetLootsByRarity(Rarity rarity)
    {
        if(_lootsByRarity == null)
        {
            _lootsByRarity = new Dictionary<Rarity, List<CardItem>>();

            foreach (var loot in DropedLoots)
            {
                if(_lootsByRarity.ContainsKey(loot.Rarity) == false)
                    _lootsByRarity[loot.Rarity] = new List<CardItem>();

                _lootsByRarity[loot.Rarity].Add(loot);
            }
        }

        return _lootsByRarity.GetValueOrDefault(rarity, new List<CardItem>());
    }
}
