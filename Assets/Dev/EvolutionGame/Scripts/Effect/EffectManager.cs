using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager
{
    private UnitBase _unit;

    private HashSet<ActiveEffect> _activeEffects = new();

    public EffectManager(UnitBase unit) 
    { 
        _unit = unit;
    }

    public void Update()
    {
        if (_activeEffects.Count == 0) return;

        foreach (var effect in _activeEffects.ToList())
        {
            if (effect.IsCompleted == false)
                effect.Update(Time.deltaTime);
            else
                _activeEffects.Remove(effect);
        }
    }

    public void AddEffect(Effect effect)
    {
        ActiveEffect findEffect = _activeEffects.Count > 0 ?
            _activeEffects.First(x => x.Effect == effect) : null;

        if (findEffect == null)
        {
            ActiveEffect activeEffect = new ActiveEffect(_unit, effect);
            _activeEffects.Add(activeEffect);
        }
        else
        {
            findEffect.RefreshDuration();
        }
    }
}

public class ActiveEffect
{
    private float _tickInterval = 1f;
    private float _tickTimer;
    private float _remainigTime;
    
    private Effect _effect;
    private UnitBase _unit;

    public Effect Effect => _effect;
    public bool IsCompleted => _remainigTime <= 0;

    public ActiveEffect(UnitBase unit, Effect effect)
    {
        _unit = unit;
        _effect = effect;
        _remainigTime = _effect.DurationTime;

        _effect.Apply(unit);
    }

    public void Update(float deltaTime)
    {
        _remainigTime -= deltaTime;

        if(_remainigTime >= 0)
        {
            _tickTimer += deltaTime;

            if(_tickTimer >= _tickInterval)
            {
                _effect.Tick(_unit);
                _tickTimer -= _tickInterval;
            }
        }
        else
        {
            _effect.Remove();
        }
    }

    public void RefreshDuration()
    {
        _remainigTime = _effect.DurationTime;
    }
}
