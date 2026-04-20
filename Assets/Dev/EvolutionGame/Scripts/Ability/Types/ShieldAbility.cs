using UnityEngine;

[CreateAssetMenu(fileName = "ShieldAbility", menuName = "Ability/ShieldAbility")]
public class ShieldAbility : Ability
{
    [SerializeField] private float _armorValue;

    public override void Activated(AbilityContext context, int level)
    {
        if(context.PlayerUnits == null || context.PlayerUnits.Count < 0) return;

        foreach(var unit in context.PlayerUnits)
        {
            ShieldEffect effect = new ShieldEffect(_armorValue + _levelUpIncrease * level);
            effect.DurationTime = 0f;

            unit.AddEffect(effect);
        }
    }
}
