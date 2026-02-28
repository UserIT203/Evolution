using UnityEngine;
using UnityEngine.AI;

public class FSMSearchTargetState : FSMState
{
    private readonly NavMeshAgent _agent;
    private readonly UnitBase _unit;

    private Collider[] _victimColliders;

    public FSMSearchTargetState(FSM fsm, NavMeshAgent agent, UnitBase unit) : base(fsm)
    {
        _agent = agent;
        _unit = unit;

        _agent.stoppingDistance = unit.UnitStats.AttackRange.GetValue() - 1f;
        
        _victimColliders = new Collider[32];
    }

    public override void Enter()
    {
        base.Enter();
        if (_agent != null)
        {
            _agent.isStopped = false;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LateUpdate()
    {
        SearchTarget();

        _agent.speed = _unit.UnitStats.Speed.GetValue();

        if (_agent != null) 
            _agent?.SetDestination(_unit.TargetBase.position);
    }

    public override void Update()
    {
        
    }

    private void SearchTarget()
    {
        _victimColliders = Physics.OverlapSphere(
            _unit.transform.position, 
            _unit.UnitStats.DetectedRange.GetValue(),
            _unit.UnitConfig.AttackMask);

        if (_victimColliders.Length <= 0) return;

        foreach (Collider victim in _victimColliders)
        {
            if (victim.TryGetComponent<IDamagaeble>(out var target) 
                && victim.transform.tag == _unit.UnitConfig.VictimTag)
            {
                _unit.SetVictim(target);
            }
        }
    }
}
