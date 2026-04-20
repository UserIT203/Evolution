using UnityEngine;
using UnityEngine.AI;

public class FSMFollowTargetState : FSMState
{
    private UnitBase _unit;
    private NavMeshAgent _agent;

    public FSMFollowTargetState(FSM fsm, NavMeshAgent agent, UnitBase unit) : base(fsm)
    {
        _agent = agent;
        _unit = unit;
    }

    public override void Enter()
    {
        _agent.isStopped = false;
    }

    public override void Update()
    {
        if (_unit.Victim != null)
            _agent.SetDestination(_unit.Victim.Transform.position);
    }
}
