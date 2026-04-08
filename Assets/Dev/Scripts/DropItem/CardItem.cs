using System;
using UnityEngine;

public class CardItem : ScriptableObject
{
    [field: Header("Identification")]
    [field: SerializeField] public string CardID { get; private set; }

    [Header("UI")]
    public LocalizeText CardName;
    public Sprite Sprite;
    public LocalizeText Description;

    [Header("Probability")]
    public Rarity Rarity;

    [field: Header("Progression")]
    [field: SerializeField] public int MaxLevel { get; private set; }
    [SerializeField] public int[] CardsRequiredPerLevel;

    private void OnValidate()
    {
        if (CardsRequiredPerLevel == null || CardsRequiredPerLevel.Length != MaxLevel)
        {
            Array.Resize(ref CardsRequiredPerLevel, MaxLevel);
            for (int i = 0; i < MaxLevel; i++)
                CardsRequiredPerLevel[i] = Mathf.Max(1, i + 2);
        }

        if (CardID == string.Empty) CardID = this.GetHashCode().ToString();
    }
}
