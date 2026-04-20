using UnityEngine;

[CreateAssetMenu(fileName = "BombAbility", menuName = "Ability/BombAbility")]
public class BombAbility : Ability
{
    [Header("Ability Configs")]
    [SerializeField] private float _bombDamage;

    public override void Activated(AbilityContext context, int level)
    {
        if(context.EnemiesUnits.Count == 0 || context.EnemiesUnits == null) return;

        float damage = _bombDamage + _levelUpIncrease * (float)level;

        for(int i = 0; i < context.EnemiesUnits.Count; i++)
            context.EnemiesUnits[i].TakeDamage(damage);
    }
}
