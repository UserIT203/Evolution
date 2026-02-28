using System.Collections;
using UnityEngine;

public class Bullet : Poolable
{
    private const float LifeTime = 4f;

    private BulletConfig _config;

    private float _damage;

    private LayerMask _hitLayerMask;
    private CustomPool<Bullet> _pool;
    private Vector3 _direction;
    private GameManager _gameManager;

    private void Update()
    {
        if (gameObject.activeSelf == true)
        {
            transform.Translate(_direction * _config.ShootForce * Time.deltaTime, Space.World);
        }
    }

    private void LateUpdate()
    {
        if (gameObject.activeSelf == true)
            OnHitTarget();
    }

    private void OnHitTarget()
    {
        Ray ray = new Ray(transform.position, _direction);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, _config.HitRadius, _hitLayerMask))
        {
            if (hitInfo.collider.TryGetComponent<IDamagaeble>(out var target))
            {
                _config.HitTarget(_damage, target);
                Release();
            }
        }
    }

    private IEnumerator OnRelease()
    {
        yield return new WaitForSeconds(LifeTime);

        Release();
    }

    public void Shoot(
        Vector3 direction,
        float damage,
        BulletConfig config,
        LayerMask triggerMask
        )
    {
        _damage = damage;
        _config = config;
        _direction = direction;
        _hitLayerMask = triggerMask;

        StartCoroutine(OnRelease());
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, _direction);
    }
#endif
}
