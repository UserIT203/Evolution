using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSetting", menuName = "Level/Setting")]
public class LevelSetting : ScriptableObject
{
    [field: SerializeField] public string LevelName;
    [field: SerializeField] public Sprite LevelIcon;
    [field: SerializeField] public GameModifier[] Modifiers;
    [field: SerializeField] public UnitBase[] PlayerUnits;
    [field: SerializeField] public UnitBase[] EnemyUnits;
    [field: SerializeField] public WavesConfig WavesConfig;
    [field: SerializeField] public LevelSpawnConfig LevelOptions;
}
