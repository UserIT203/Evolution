using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest", menuName = "Chest/ChestConfig")]
public class ChestConfig : ScriptableObject, IPurchasedItem
{
    [field: SerializeField] public int Price { get; set; }
    [field: SerializeField] public Sprite CloseIcon { get; set; }
    [field: SerializeField] public Sprite OpenIcon { get; set; }
    [field: SerializeField] public bool UseDonatMoney { get; set; }

    public Rarity ChestRarity;
    public int MinItems;
    public int MaxItems;

    public List<RarityWeigth> RarityRollWeigth = new List<RarityWeigth>();
}

[System.Serializable]
public class RarityWeigth
{
    public Rarity Rarity;
    [Range(0f, 100f)] public float Weigth = 1f;
}
