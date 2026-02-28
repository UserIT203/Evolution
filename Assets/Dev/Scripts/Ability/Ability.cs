using UnityEngine;

public abstract class Ability : CardItem
{
    [SerializeField] protected float _levelUpIncrease;

    public abstract void Activated(AbilityContext context, int level);
}