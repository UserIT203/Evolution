using UnityEngine;

public class FreezeEffect : Effect
{
    private float _originSpeedValue;
    private float _originDamageValue;

    private UnitBase _unit;

    public override void Apply(UnitBase unit)
    {
        _unit = unit;

        _originSpeedValue = unit.UnitStats.Speed.GetValue();
        _originDamageValue = unit.UnitStats.Damage.GetValue();

        unit.FreezeAction(0f, 0f);
    }

    public override void Remove()
    {
        _unit.FreezeAction(_originSpeedValue, _originDamageValue);
    }
}
