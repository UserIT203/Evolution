
public class ItemManager
{
    public ItemUseContext ItemContext { get; set; }

    public void Initiliazed(LevelUpgrade upgrade) => ItemContext = new ItemUseContext(upgrade);
}
