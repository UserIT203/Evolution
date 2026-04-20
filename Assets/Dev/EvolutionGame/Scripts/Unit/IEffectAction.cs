using UnityEngine;

public interface IEffectAction
{
    public void FreezeAction(float speedValue, float healthValue);

    public void ShieldAction(float shieldValue);
}
