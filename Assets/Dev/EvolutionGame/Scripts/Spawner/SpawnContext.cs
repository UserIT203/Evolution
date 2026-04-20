using UnityEngine;

public struct SpawnContext
{
    public Transform TowerTransform;
    public CustomPool<Bullet> BulletPool;
    public GameManager GameManager;
    public GlobalManager GlobalManager;
}
