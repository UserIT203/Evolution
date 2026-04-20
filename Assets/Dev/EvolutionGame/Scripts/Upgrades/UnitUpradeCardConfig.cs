using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitUpradeCard", menuName = "Unit/Uprade Card")]
public class UnitUpradeCardConfig : CardItem
{
    [Header("Upgrade Options")]
    [SerializeField] public Modifier BaseHealthModifier;
    [SerializeField] public Modifier BaseDamageModifier;
    [SerializeField] public Modifier BaseSpeedModifier;

    public Modifier GetScaledModifier(Modifier baseMod, int level)
    {
        return level > 0 ?
            new Modifier { ModifierValue = baseMod.ModifierValue * level } :
            new Modifier { ModifierValue = baseMod.ModifierValue };
    }
}
