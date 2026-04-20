using UnityEngine;

[CreateAssetMenu(fileName = "UnitRangedConfig", menuName = "Unit/RangedConfig")]
public class UnitRangedConfig : UnitStatsConfig
{
    [field: Header("Ranged Options")]
    [field: SerializeField] public BulletConfig BulletConfig { get; private set; }
}
