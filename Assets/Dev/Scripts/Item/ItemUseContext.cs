using UnityEngine;

public class ItemUseContext
{
    private IItemHandler _levelUpgrades;

    public ItemUseContext(IItemHandler levelUpgrades)
    {
        _levelUpgrades = levelUpgrades;
    }

    public IItemHandler GetHandler(ItemHandlerType type)
    {
        return type switch
        {
            ItemHandlerType.UpgradeUnit => _levelUpgrades,
            _ => null
        };
    }
}

public enum ItemHandlerType
{
    UpgradeUnit,
    BuySkillUpgrades
}
