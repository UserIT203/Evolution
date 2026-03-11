using System;
using System.Collections.Generic;
using UnityEngine;

public class FSMState
{
    public HashSet<ITransition> Transitions { get; private set; }
    protected readonly FSM _fsm;

    public FSMState(FSM fsm)
    {
        _fsm = fsm;
        Transitions = new HashSet<ITransition>();
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }

    public virtual void LateUpdate() { }

    public void AddTransition(FSMState state, IPredicate condition)
    {
        Transitions.Add(new Transition(state, condition));
    }
}



#region Predicate
public interface IPredicate
{
    bool Evaluate();
}

public class FuncPredicate : IPredicate
{

    public readonly Func<bool> _func;

    public FuncPredicate(Func<bool> func)
    {
        _func = func;
    }

    public bool Evaluate() => _func.Invoke();
}
#endregion

#region Transition
public interface ITransition
{
    FSMState TargetState { get; }
    IPredicate Predicate { get; }
}

public class Transition : ITransition
{
    public FSMState TargetState { get; }

    public IPredicate Predicate { get; }

    public Transition(FSMState targetTransition, IPredicate predicate)
    {
        TargetState = targetTransition;
        Predicate = predicate;
    }
}
#endregion
