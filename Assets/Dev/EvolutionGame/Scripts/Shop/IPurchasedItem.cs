using UnityEngine;

public interface IPurchasedItem
{
    public Sprite CloseIcon { get; set; }
    public int Price { get; set; }
    public bool UseDonatMoney { get; set; }
}
