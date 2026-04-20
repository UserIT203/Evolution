using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBus
{
    private static SpawnBus _instance;

    public static SpawnBus GetInstance()
    {
        if (_instance == null)
            _instance = new SpawnBus();

        return _instance;
    }

    public List<UnitBase> ActiveEnemiesUnits = new();
    public List<UnitBase> ActivePlayerUnits = new();

    public Action<GameManager, UnitType> onSpawnPlayerUnit;
    public Action<WaveManager, UnitType> onSpawnEnemyUnit;
}
