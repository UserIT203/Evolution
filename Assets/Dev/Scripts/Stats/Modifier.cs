using UnityEngine;

[System.Serializable]
public struct Modifier
{
    [field: SerializeField] public ModifierType ModifierType;
    [field: SerializeField] public float ModifierValue { get; set; }
}

public enum ModifierType
{
    Health,
    Damage,
    Speed
}