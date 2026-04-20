using UnityEngine;
using System;

public interface ICollectedCard
{
    public bool TryUpgrade(string id);
    public int GetLevel(string id);
    public int GetCollectedCards(string id);
    public int GetCardsNeededForNextLevel(string id);
    public CardItem[] GetActiveCards();
}
