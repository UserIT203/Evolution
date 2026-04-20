using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class EnviroumentEntryPoint : Bootstrap
{
    [SerializeField] private LevelBuilder _levelBuilder;
    [SerializeField] private UnitSpawner _unitSpawner;

    public override async UniTask Initialized(ISceneArgs args)
    {
        Debug.Log("<color=yellow>Start Init Enviroument Bootstrap</color>");

        if (args is not EnviroumentArgs enviroumentArgs)
            throw new ArgumentException("Invalidet Type");

        _levelBuilder.UnitSpawner = _unitSpawner;
        _levelBuilder.GameManager = enviroumentArgs.GameManager;

        _levelBuilder.Initialized();

        _levelBuilder.SetBuildSettings(enviroumentArgs.LevelConfigs);
        await _levelBuilder.SpawnLevel(enviroumentArgs.LevelIndex);

        _unitSpawner.GlobalManager = enviroumentArgs.GlobalManager;
        _unitSpawner.GameManager = enviroumentArgs.GameManager;

        _unitSpawner.Initialized(enviroumentArgs.LevelSetting);

        Debug.Log("<color=green>End Init Enviroument Bootstrap</color>");
    }
}
