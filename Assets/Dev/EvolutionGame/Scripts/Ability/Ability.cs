using UnityEngine;

public abstract class Ability : CardItem
{
    [Header("<color=yellow>Ability Settings</color>")]
    [field: SerializeField] public float DelayTime { get; private set; }
    [SerializeField] protected float _levelUpIncrease;

    public abstract void Activated(AbilityContext context, int level);
}