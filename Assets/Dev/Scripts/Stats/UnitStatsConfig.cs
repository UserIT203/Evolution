using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsConfig", menuName = "Unit/StatConfig")]
public class UnitStatsConfig : ScriptableObject
{
    [field: SerializeField] public LocalizeText UnitName { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public UnitType UnitType { get; private set; }

    [field: Header("Main Stats")]
    [field: SerializeField] public float Maxhealth { get; private set; }
    [field: SerializeField] public float Armor { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }

    [field: Header("Attack Options")]
    [field: SerializeField] public float AttackDelay { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float DetectedRange { get; private set; }

    [field: Header("Other Options")]

    [field:SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public bool IsUnlock { get; private set; }
    [field: SerializeField] public int UnlockCosts { get; private set; }

    public void UnlockUnit() => IsUnlock = true;
}

public enum UnitType
{
    Melee, 
    Ranged,
    Heavy
}