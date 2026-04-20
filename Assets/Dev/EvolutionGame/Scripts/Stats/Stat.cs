using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stat
{
    public float BaseValue;
    
    [SerializeField] private List<Modifier> _modifiers;

    public float GetValue()
    {
        if(BaseValue == 0f) return 0f;

        if(_modifiers == null)
            _modifiers = new List<Modifier>();

        float value = 0f;

        foreach (Modifier modifier in _modifiers)
        {
            value += modifier.ModifierValue;
        }

        return value + BaseValue;
    }

    public void AddModifier(Modifier modifier)
    {
        _modifiers.Add(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        if(_modifiers.Contains(modifier))
            _modifiers.Remove(modifier);
    }

    public void RemoveAllModifier()
    {
        if(_modifiers.Count == 0) return;

        _modifiers.Clear();
    }
}
