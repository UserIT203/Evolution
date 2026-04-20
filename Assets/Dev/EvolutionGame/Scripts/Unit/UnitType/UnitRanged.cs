using UnityEngine;

public class UnitRanged : UnitBase
{
    [SerializeField] private Transform _shootPoint;

    private UnitRangedConfig _rangedConfig;

    protected override void Initialized()
    {
        _rangedConfig = _unitConfig as UnitRangedConfig;
        base.Initialized();
    }

    protected override void InitializedFSM()
    {
        _fsm.AddFsm(new FSMSearchTargetState(_fsm, _agent, this));
        _fsm.AddFsm(new FSMRangedAttack(_fsm, this, _agent, _rangedConfig.BulletConfig, _shootPoint));
        _fsm.AddFsm(new FSMFollowTargetState(_fsm, _agent, this));

        _fsm.AddTransition<FSMSearchTargetState, FSMFollowTargetState>(
            new FuncPredicate(HasFollowTarget));

        _fsm.AddTransition<FSMFollowTargetState, FSMSearchTargetState>(
            new FuncPredicate(HasSearchTarget));
        _fsm.AddTransition<FSMFollowTargetState, FSMRangedAttack>(
            new FuncPredicate(HasAttack));

        _fsm.SetState<FSMSearchTargetState>();
    }
}
