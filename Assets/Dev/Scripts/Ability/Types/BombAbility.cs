using UnityEngine;

[CreateAssetMenu(fileName = "BombAbility", menuName = "Ability/BombAbility")]
public class BombAbility : Ability
{
    [Header("Ability Configs")]
    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private Stat _bombDamage;

    public override void Activated(AbilityContext context, int level)
    {
        if (context.BombAbilityPosition == null) return;

        Collider[] hitTarget = Physics.OverlapSphere(context.BombAbilityPosition.position, 10f, _enemyMask);

        foreach (var hit in hitTarget)
        {
            if(hit.TryGetComponent<IDamagaeble>(out var unit))
            {
                unit.TakeDamage(_bombDamage.GetValue() + _levelUpIncrease * level);
            }
        }
    }
}
