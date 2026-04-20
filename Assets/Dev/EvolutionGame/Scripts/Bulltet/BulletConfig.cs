using UnityEngine;

[CreateAssetMenu(fileName = "BulletBonfig", menuName = "Bullet/Default")]
public class BulletConfig : ScriptableObject
{
    public float ShootForce;
    public float HitRadius;

    public virtual void HitTarget(float damage, IDamagaeble target = null)
    {
        target.TakeDamage(damage);
    }
}
