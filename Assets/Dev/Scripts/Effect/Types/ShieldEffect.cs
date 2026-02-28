using UnityEngine;

public class ShieldEffect : Effect
{
    private float _armorValue;

    public ShieldEffect(float armorValue)
    {
        _armorValue = armorValue;
    }


    public override void Apply(UnitBase unit)
    {
        unit.ShieldAction(_armorValue);
    }

    public override void Remove()
    {
        throw new System.NotImplementedException();
    }
}
