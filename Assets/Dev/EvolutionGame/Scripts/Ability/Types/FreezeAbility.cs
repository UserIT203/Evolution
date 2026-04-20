using UnityEngine;

[CreateAssetMenu(fileName = "FreezeAbility", menuName = "Ability/FreezeAbility")]
public class FreezeAbility : Ability
{
    [SerializeField] private float _timeToActive;

    public override void Activated(AbilityContext context, int level)
    {
        if(context.EnemiesUnits == null) return;

        foreach (var unit in context.EnemiesUnits)
        {
            Effect freezeEffect = new FreezeEffect();
            freezeEffect.DurationTime = _timeToActive + _levelUpIncrease * level;
            unit.AddEffect(freezeEffect);
        }
            
        Debug.Log($"Enemies Units Count {context.EnemiesUnits.Count}");
    }
}
