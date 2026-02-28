using UnityEngine;
using UnityEngine.AI;

public class FSMMeleeAttack : FSMState
{
    protected readonly NavMeshAgent _agent;
    protected readonly UnitBase _unit;

    protected float _attackTimer;

    public FSMMeleeAttack(FSM fsm, UnitBase unit, NavMeshAgent agent) : base(fsm)
    {
        _unit = unit;
        _agent = agent;
    }

    public override void Enter()
    {
        base.Enter();

        if (_agent != null)
            _agent.isStopped = true;
    }

    public override void Update()
    {
        _attackTimer -= Time.deltaTime;
        Attack();
    }

    public override void LateUpdate()
    {
        if (_unit.Victim == null)
            _fsm.SetState<FSMSearchTargetState>();

        if (_unit.Victim != null)
        {
            RotationToTarget();
        }
    }

    protected virtual void Attack()
    {
        if (_attackTimer > 0) return;

        _unit.Victim?.TakeDamage(_unit.UnitStats.Damage.GetValue());
        _attackTimer = _unit.UnitStats.AttackDelay.GetValue();

        _unit.Attack();
    }

    private void RotationToTarget()
    {
        Vector3 direction = _unit.Victim.Transform.position - _unit.transform.position;
        direction.y = 0;

        if(direction != Vector3.zero)
        {
            Quaternion targetRotaion = Quaternion.LookRotation(direction);
            _unit.transform.rotation = Quaternion.Slerp(
                _unit.transform.rotation,
                targetRotaion,
                2f * Time.deltaTime
                );
        }
    }
}
