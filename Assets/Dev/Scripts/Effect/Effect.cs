using UnityEngine;

public abstract class Effect
{
    public float DurationTime;
    
    public abstract void Apply(UnitBase unit);
    
    public abstract void Remove();

    public virtual void Tick(UnitBase unit)
    {

    }
}
