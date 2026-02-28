using UnityEngine;
using Zenject;

public class ItemManager : MonoBehaviour
{
    public ItemUseContext ItemContext { get; private set; }

    [Inject]
    public void Construct(LevelUpgrade levelUpgradeHandler)
    {
        ItemContext = new ItemUseContext(levelUpgradeHandler);
    }
}
