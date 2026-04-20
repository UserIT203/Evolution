using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;
using System.Linq;

public class UnitSpawner : MonoBehaviour
{
    [Inject] private ItemManager _itemManager;

    [Header("SpawnPoint Units")]
    [SerializeField] private Transform _playerUnitSpawnPoint;
    [SerializeField] private Transform _enemyUnitSpawnPoint;

    [Header("Towers")]
    [SerializeField] private Transform _playerTower;
    [SerializeField] private Transform _enemyTower;

    [Header("Spawn Options")]
    [SerializeField] private Vector3 _spawnOffset;
    [SerializeField] private Transform _unitPool;

    [Header("Bullet Pool")]
    [SerializeField] private Bullet _bulletTemplate;
    [SerializeField] private Transform _bulletContainer;

    private CustomPool<Bullet> _bulletPool;

    private SpawnContext _enemySpawnContext;
    private SpawnContext _playerSpawnContext;

    private Dictionary<UnitType, CustomPool<UnitBase>> _playerDictianory;
    private Dictionary<UnitType, CustomPool<UnitBase>> _enemyDictianory;

    public GlobalManager GlobalManager;
    public GameManager GameManager;

    private void OnEnable()
    {
        SpawnBus.GetInstance().onSpawnEnemyUnit += SpawnUnit;
        SpawnBus.GetInstance().onSpawnPlayerUnit += SpawnUnit;
    }

    private void OnDisable()
    {
        SpawnBus.GetInstance().onSpawnEnemyUnit -= SpawnUnit;
        SpawnBus.GetInstance().onSpawnPlayerUnit -= SpawnUnit;

        SpawnBus.GetInstance().ActivePlayerUnits.Clear();
        SpawnBus.GetInstance().ActiveEnemiesUnits.Clear();
    }

    public void Initialized(LevelSetting levelSettings)
    {
        SetEnemyUnit(levelSettings);
        SetPlayerUnit(GameManager.UnitInfo.ToList());
    }

    private void CreateUnitDictionary(
        UnitBase[] units, 
        ref Dictionary<UnitType, CustomPool<UnitBase>> unitDictianory)
    {
        if(unitDictianory != null && unitDictianory.Count != 0)
        {
            foreach(var pool in unitDictianory.Values)
                pool.ClearAll();
        }

        unitDictianory = new Dictionary<UnitType, CustomPool<UnitBase>>();

        foreach (var unit in units)
        {
            if (unitDictianory.ContainsKey(unit.UnitConfig.UnitType) == false)
            {
                CustomPool<UnitBase> pool = new CustomPool<UnitBase>(unit, 15, _unitPool);
                unitDictianory.Add(unit.UnitConfig.UnitType, pool);
            }
        }
    }

    public void SetTowers(Transform enemyTower, Transform playerTower)
    {
        Debug.LogWarning($"Set Tower in Spawner {enemyTower}");

        _enemyTower = enemyTower;
        _playerTower = playerTower;
    }

    public void SpawnUnit(WaveManager waveManager, UnitType unitType)
    {
        UnitBase unit = _enemyDictianory[unitType].Get();
        unit.Spawn(_enemySpawnContext);

        Vector3 randomOffset = new Vector3
            (
                UnityEngine.Random.Range(-_spawnOffset.x, _spawnOffset.x),
                0f,
                UnityEngine.Random.Range(-_spawnOffset.z, _spawnOffset.z)
            );

        unit.GetComponent<LootBag>().Initialized(_itemManager.ItemContext);

        unit.transform.position = _enemyUnitSpawnPoint.position + randomOffset;
        unit.gameObject.SetActive(true);

        TrackedActiveUnit(unit, SpawnBus.GetInstance().ActiveEnemiesUnits);
    }

    public void SpawnUnit(GameManager player, UnitType unitType)
    {
        UnitBase unit = _playerDictianory[unitType].Get();
        unit.Spawn(_playerSpawnContext);

        Vector3 randomOffset = new Vector3
            (
                UnityEngine.Random.Range(-_spawnOffset.x, _spawnOffset.x),
                0f,
                UnityEngine.Random.Range(-_spawnOffset.z, _spawnOffset.z)
            );

        unit.transform.position = _playerUnitSpawnPoint.position + randomOffset;
        unit.gameObject.SetActive(true);

        TrackedActiveUnit(unit, SpawnBus.GetInstance().ActivePlayerUnits);

        if (unit.UnitConfig.UnitType == UnitType.Melee)
            QuestBus.GetInstance().onUpdateCounter?.Invoke(QuestType.SpawnMelleUnits, 1);

        if (unit.UnitConfig.UnitType == UnitType.Ranged)
            QuestBus.GetInstance().onUpdateCounter?.Invoke(QuestType.SpawnRangedUnits, 1);

        if (unit.UnitConfig.UnitType == UnitType.Heavy)
            QuestBus.GetInstance().onUpdateCounter?.Invoke(QuestType.SpawnHeavyUnits, 1);

        QuestBus.GetInstance().onUpdateCounter?.Invoke(QuestType.SpawnUnits, 1);
    }

    private void TrackedActiveUnit(UnitBase unit, List<UnitBase> list)
    {
        list.Add(unit);

        Action handle = null;
        handle = () =>
        {
            list.Remove(unit);
            unit.onRelease -= handle;
        };

        unit.onRelease += handle;
    }

    public void SetEnemyUnit(LevelSetting levelSetting)
    {
        Debug.LogWarning($"Create Dictianory for enemy\n{_enemyTower}");

        CreateUnitDictionary(levelSetting.EnemyUnits, ref _enemyDictianory);

        _bulletPool = _bulletPool ?? new CustomPool<Bullet>(_bulletTemplate, 30, _bulletContainer);

        _enemySpawnContext = new SpawnContext
        {
            TowerTransform = _playerTower,
            BulletPool = _bulletPool,
            GameManager = GameManager,
            GlobalManager = null
        };
    }

    public void SetPlayerUnit(List<UnitInfo> unitsInfo)
    {
        Debug.LogWarning($"Create Dictianory for player\n{_playerTower}");

        List<UnitBase> unlockUnits = new();

        foreach (var unit in unitsInfo)
        {
            if (unit.IsUnlock == true)
                unlockUnits.Add(unit.Unit);
        }

        CreateUnitDictionary(unlockUnits.ToArray(), ref _playerDictianory);

        _bulletPool = _bulletPool ?? new CustomPool<Bullet>(_bulletTemplate, 30, _bulletContainer);

        _playerSpawnContext = new SpawnContext
        {
            TowerTransform = _enemyTower,
            BulletPool = _bulletPool,
            GameManager = GameManager,
            GlobalManager = GlobalManager
        };
    }
}
