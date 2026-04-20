using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private FSMState _currentState;
    private Dictionary<Type, FSMState> _states = new Dictionary<Type, FSMState>();

    public FSMState CurrentState => _currentState;

    public void AddFsm(FSMState state)
    {
        _states.Add(state.GetType(), state);
    }

    public void SetState<T>() where T : FSMState
    {
        var type = typeof(T);
        ChangeState(type);
    }

    public void SetState(FSMState state)
    {
        ChangeState(state.GetType());
    }

    public void AddTransition<T, U>(IPredicate predicate)
        where T : FSMState
        where U : FSMState
    {
        FSMState mainState = GetState(typeof(T));
        FSMState targetState = GetState(typeof(U));

        mainState.AddTransition(targetState, predicate);
    }

    public void Update()
    {
        CheckTransition();
        _currentState?.Update();
    }

    public void LateUpdate()
    {
        _currentState?.LateUpdate();
    }

    private void ChangeState(Type type)
    {
        if (_currentState != null && _currentState.GetType() == type) return;

        FSMState newState = GetState(type);

        if (newState == null) return;

        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private FSMState GetState(Type type)
    {
        if (_states.TryGetValue(type, out var newState))
            return newState;

        return null;
    }

    private void CheckTransition()
    {
        var transition = GetTransition();

        if (transition != null)
        {
            var state = transition.TargetState.GetType();
            ChangeState(state);
        }
    }

    private ITransition GetTransition()
    {
        if (_currentState?.Transitions == null) return null;

        foreach (var transition in _currentState?.Transitions)
        {
            if (transition.Predicate.Evaluate())
                return transition;
        }

        return null;
    }
}
