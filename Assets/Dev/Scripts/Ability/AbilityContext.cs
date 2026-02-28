using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct AbilityContext
{
    public Transform BombAbilityPosition;
    public IReadOnlyList<UnitBase> EnemiesUnits;
    public IReadOnlyList<UnitBase> PlayerUnits;
}
