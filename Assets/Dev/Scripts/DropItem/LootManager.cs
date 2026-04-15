using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;

public class LootManager : MonoBehaviour
{
    [Inject] private GlobalManager _globalManager;
    [Inject] private AbilityManager _abilityManager;

    [Header("<color=green>Main Links</color>")]
    [SerializeField] private AllLootPool _droppedLoots;
    [Space(5f)]
    [Header("<color=yellow>Drop One Card Settings</color>")]
    [SerializeField] private List<RarityWeigth> _rarityWeigth;

    public AllLootPool DroppedLoots => _droppedLoots;

    public void LootHandler(CardItem loot)
    {
        Debug.Log($"Add new card {loot}");
        
        switch (loot)
        {
            case UnitUpradeCardConfig upgrade:
                _globalManager.CollectCard(upgrade);
                break;
            case Ability ability:
                _abilityManager.CollectedAbilityCard(ability);
                break;
            default:
                Debug.Log("None Loot Type Handler");
                break;
        }
    }

    public CardItem GetItemById(string id) => _droppedLoots.GetLootById(id);

    public void GetOneModifierCard()
    {
        float roll = Random.Range(0f, 100f);
        Rarity droppedRarity = Rarity.Common;

        foreach (var weigth in _rarityWeigth)
        {
            if (roll <= weigth.Weigth)
                droppedRarity = weigth.Rarity;
        }

        var accessDrop = _droppedLoots.GetLootsByRarity(droppedRarity);

        List<CardItem> droppedCard = new();

        foreach (var drop in accessDrop)
        {
            if(drop as UnitUpradeCardConfig)
                droppedCard.Add(drop);
        }

        int randomIndex = Random.Range(0, droppedCard.Count);

        LootHandler(droppedCard[randomIndex]);
    }
}
