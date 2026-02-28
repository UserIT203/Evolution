using UnityEngine;
using Zenject;

public class LootManager : MonoBehaviour
{
    [Inject] private GlobalManager _globalManager;
    [Inject] private AbilityManager _abilityManager;

    [SerializeField] private AllLootPool _droppedLoots;

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
}
