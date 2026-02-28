using UnityEngine;
using UnityEngine.AI;

public class FSMRangedAttack : FSMMeleeAttack
{
    private Transform _shootPoint;
    private BulletConfig _bulletConfig;

    public FSMRangedAttack(FSM fsm, UnitBase unit, NavMeshAgent agent, BulletConfig bulletConfig, Transform shootPoint) : base(fsm, unit, agent)
    {
        _shootPoint = shootPoint;
        _bulletConfig = bulletConfig;
    }

    protected override void Attack()
    {
        if (_unit.Victim == null)
            _fsm.SetState<FSMFollowTargetState>();

        if (_attackTimer > 0) return;

        Vector3 direction = (_unit.Victim.Transform.position - _unit.transform.position).normalized;

        Bullet bullet = _unit.GetBulletPool().Get();

        bullet.transform.position = _shootPoint.position;
        bullet.transform.localRotation = Quaternion.identity;
        bullet.gameObject.SetActive(true);

        bullet.Shoot(
            direction,
            _unit.UnitStats.Damage.GetValue(),
            _bulletConfig,
            _unit.UnitConfig.AttackMask);

        _attackTimer = _unit.UnitStats.AttackDelay.GetValue();
        _unit.Attack();
    }
}
