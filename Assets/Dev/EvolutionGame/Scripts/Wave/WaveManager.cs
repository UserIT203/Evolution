using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Zenject;
using System;

public class WaveManager : MonoBehaviour, ILevelHandler
{
    [SerializeField] private WavesConfig _waveConfig;

    private int _currentWaveIndex = 0;
    private Wave _currentWave;
    private Coroutine _spawnCoroutine;
    private Coroutine _waveCoroutine;

    private GameManager _gameManager;

    public WavesConfig WaveConfig => _waveConfig;
    public List<Wave> Waves => _waveConfig.Waves;

    public Action<int> onStartWave;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;

        SpawnBus.GetInstance().onCanSpawn += SetWave;
        _gameManager.onEnd += RestartWaves;
    }

    private void OnDestroy()
    {
        SpawnBus.GetInstance().onCanSpawn -= SetWave;
        _gameManager.onEnd -= RestartWaves;
    }

    private IEnumerator SetNextWave()
    {
        if(_currentWaveIndex + 1 < _waveConfig.Waves.Count)
        {
            yield return new WaitForSeconds(_currentWave.Delay);

            _currentWaveIndex++;
            SetWave();
        }
        else
        {
            Debug.Log("Wave End");

            yield return null;
        }
    }

    private IEnumerator PlayWave()
    {
        Debug.Log(_currentWave.Stages.Count);

        while(_currentWave.Stages.Count > 0)
        {
            Debug.Log($"Stages count {_currentWave.Stages.Count}");

            WaveStage stage = _currentWave.GetStage();

            foreach (var units in stage.StageUnits)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    SpawnBus.GetInstance().onSpawnEnemyUnit?.Invoke(this, units.UnitType);
                }
            }

            yield return new WaitForSeconds(stage.Delay);
        }

        _waveCoroutine = StartCoroutine(SetNextWave());

        yield return null;
    }

    public void SetWave()
    {
        _currentWave = _waveConfig.Waves[_currentWaveIndex];
        _currentWave.Initialized();

        onStartWave?.Invoke(_currentWaveIndex);

        _spawnCoroutine = StartCoroutine(PlayWave());
    }

    public void RestartWaves()
    {
        _currentWaveIndex = 0;

        if(_waveCoroutine != null) StopCoroutine(_waveCoroutine);
        if(_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
    }

    public void ChangeLevel(WavesConfig config)
    {
        _waveConfig = config;
    }

    public void SetLevelSettings(LevelSetting levelSettings)
    {
        _waveConfig = levelSettings.WavesConfig;
    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        
    }
}
