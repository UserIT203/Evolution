using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct AbilityContext
{
    public IReadOnlyList<UnitBase> EnemiesUnits;
    public IReadOnlyList<UnitBase> PlayerUnits;
}
