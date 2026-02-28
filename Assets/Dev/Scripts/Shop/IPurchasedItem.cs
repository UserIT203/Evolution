using UnityEngine;

public interface IPurchasedItem
{
    public Sprite Icon { get; set; }
    public int Price { get; set; }
    public bool UseDonatMoney { get; set; }
}
