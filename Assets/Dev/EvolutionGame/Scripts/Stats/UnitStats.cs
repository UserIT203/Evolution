using UnityEngine;

public class UnitStats
{
    public Stat MaxHealth;
    public Stat Armor;
    public Stat Speed;
    public Stat AttackDelay;
    public Stat AttackRange;
    public Stat Damage;
    public Stat DetectedRange;

    public void SetValues(UnitStatsConfig config)
    {
        MaxHealth = new Stat { BaseValue = config.Maxhealth };
        Speed = new Stat { BaseValue = config.Speed };
        Damage = new Stat{BaseValue = config.Damage };
        Armor = new Stat { BaseValue = config.Armor };
        AttackDelay = new Stat { BaseValue = config.AttackDelay };
        AttackRange = new Stat { BaseValue = config.AttackRange };
        DetectedRange = new Stat { BaseValue= config.DetectedRange };

        Debug.Log("Init Stats");
    }

    public void ApplyMultiplier(GlobalManager globalManager)
    {
        if(globalManager == null) return;

        MaxHealth.BaseValue *= globalManager.HealthMultiplier.GetValue();
        Speed.BaseValue *= globalManager.SpeedMultiplier.GetValue();
        Damage.BaseValue *= globalManager.DamageMultiplier.GetValue();
    }
}
