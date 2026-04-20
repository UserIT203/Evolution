using TMPro;
using UnityEngine;

public class UnitMelee : UnitBase
{
    protected override void InitializedFSM()
    {
        _fsm.AddFsm(new FSMSearchTargetState(_fsm, _agent, this));
        _fsm.AddFsm(new FSMMeleeAttack(_fsm, this, _agent));
        _fsm.AddFsm(new FSMFollowTargetState(_fsm, _agent, this));

        _fsm.AddTransition<FSMSearchTargetState, FSMFollowTargetState>(
            new FuncPredicate(HasFollowTarget));

        _fsm.AddTransition<FSMFollowTargetState, FSMSearchTargetState>(
            new FuncPredicate(HasSearchTarget));
        _fsm.AddTransition<FSMFollowTargetState, FSMMeleeAttack>(
            new FuncPredicate(HasAttack));

        _fsm.SetState<FSMSearchTargetState>();
    }
}
