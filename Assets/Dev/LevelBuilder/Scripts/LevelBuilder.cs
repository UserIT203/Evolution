using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using Zenject;

public class LevelBuilder : MonoBehaviour
{
    [Inject] private DiContainer _DIContainer;

    [Header("Main Settings")]
    [SerializeField] private LevelGridConfig _gridConfig;
    [SerializeField] private Vector3 _levelCenter;
    [SerializeField] private List<LevelOptions> _levels;
    [SerializeField] private List<LevelRoot> _levelsRoots;

    [Header("Props Settings")]
    [SerializeField] private int _propsPerCellMin = 1;
    [SerializeField] private int _propsPerCellMax = 5;
    [SerializeField] private float propsScatterRadius = 0.4f;

    private UnitSpawner _unitSpawner;

    private int _cellSize;
    private int _currentLevelOrder = int.MaxValue;

    public LevelSpawnConfig FocusLevelConfig
    {
        get
        {
            if(_levels != null && _currentLevelOrder < _levels.Count)
                return _levels[_currentLevelOrder].Config;
            else
                return null;
        }
    }

    [Inject]
    public void Construct(UnitSpawner unitSpawner)
    {
        _unitSpawner = unitSpawner;

        if (_levelsRoots.Count == 0)
        {
            SpawnLevels();
        }
    }

    public void SetBuildSettings(List<LevelSpawnConfig> levelsConfigs)
    {
        if(_levels.Count > 0)
            _levels.Clear();

        if (_levelsRoots.Count > 0)
            ClearAll();

        for (int i = 0; i < levelsConfigs.Count; i++)
        {
            LevelOptions newOption = new LevelOptions
            {
                Order = i,
                Config = levelsConfigs[i]
            };

            _levels.Add(newOption);
        }

        SpawnLevels();
    }

    public void ClearAll()
    {
        if (_levelsRoots.Count <= 0) return;

        foreach (var levelRoot in _levelsRoots)
        {
            if(transform.GetChild(0).gameObject != null)
                DestroyImmediate(transform.GetChild(0).gameObject, false);
        }

        _currentLevelOrder = int.MaxValue;
        _levelsRoots.Clear();
    }

    public void SetNextLevel()
    {
        int newOrder = Mathf.Clamp(_currentLevelOrder + 1, 0, _levelsRoots.Count - 1);
        SetActiveLevel(newOrder);
    }

    public void SetPreviousLevel()
    {
        int newOrder = Mathf.Clamp(_currentLevelOrder - 1, 0, _levelsRoots.Count - 1);
        SetActiveLevel(newOrder);
    }

    public void SpawnLevels()
    {
        foreach (LevelOptions level in _levels)
        {
            GameObject levelRoot = new GameObject($"LevelRoot{level.Order}");
            levelRoot.transform.SetParent(transform, false);

            LevelRoot newLevelLinks = new LevelRoot {
                Order = level.Order,
                ChildIndex = level.Order
            };

            _levelsRoots.Add(newLevelLinks);

            SpawnLevelObjects(level, newLevelLinks);
        }

        EditorUtility.SetDirty(this);

        SetActiveLevel(0);
    }

    public void RebuildSelectedLevel()
    {
        if (_levelsRoots.Exists(item => item.Order == _currentLevelOrder) == true)
        {
            LevelRoot levelLinks = _levelsRoots.Find(item => item.Order == _currentLevelOrder);

            LevelOptions levelOptions = _levels.Find(l => l.Order == _currentLevelOrder);

            _levelsRoots.Remove(levelLinks);
            DestroyImmediate(transform.GetChild(levelLinks.ChildIndex).gameObject, false);

            GameObject levelRoot = new GameObject($"LevelRoot{levelOptions.Order}");
            levelRoot.transform.SetParent(transform, false);
            levelRoot.transform.SetSiblingIndex(levelLinks.ChildIndex);

            LevelRoot newLevelLinks = new LevelRoot
            {
                Order = levelOptions.Order,
                ChildIndex = levelLinks.ChildIndex,
            };

            _levelsRoots.Add(newLevelLinks);

            SpawnLevelObjects(levelOptions, newLevelLinks);

            EditorUtility.SetDirty(this);
        }
    }

    private void SetActiveLevel(int index)
    {
        HideAllLevels();

        if (_levelsRoots.Exists(item => item.Order == index) == true)
        {
            LevelRoot levelLinks = _levelsRoots.Find(item => item.Order == index);

            _currentLevelOrder = index;
            transform.GetChild(levelLinks.ChildIndex).gameObject.SetActive(true);

            SetTowers();
        }
    }

    private void HideAllLevels()
    {
        if (_levelsRoots.Count <= 0) return;

        foreach (var level in _levelsRoots)
        {
            transform.GetChild(level.ChildIndex).gameObject.SetActive(false);
        }
    }

    private void SpawnLevelObjects(LevelOptions levelOptions, LevelRoot levelRoot)
    {
        var cell = _gridConfig.GetCellObjects();
        var objectByType = levelOptions.Config.TypeObjects;
        _cellSize = _gridConfig.CellSize;

        int childIndex = _levelsRoots.Find(item => item.Order == levelOptions.Order).ChildIndex;
        Transform root = transform.GetChild(childIndex);

        SpawnPlane(objectByType[LevelTypeObject.Plane][0], root);

        int index = 0;

        for(int row = 0; row < _gridConfig.Row; row++)
        {
            for (int col = 0; col < _gridConfig.Col; col++)
            {
                if (index >= cell.Count) return;

                float halfWidth = (_gridConfig.Col - 1) * _cellSize / 2f;
                float halfDepth = (_gridConfig.Row - 1) * _cellSize / 2f;

                Vector3 cellCenter = new Vector3(
                    _levelCenter.x + (col * _cellSize - halfWidth),
                    0f,
                    _levelCenter.z + (row * _cellSize - halfDepth)
                    );

                switch (cell[index])
                {
                    case LevelTypeObject.Props:
                        SpawnProps(cellCenter, objectByType[cell[index]], root);
                        break;
                    
                    case LevelTypeObject.PlayerTower :
                        levelRoot.PlayerTower = 
                            SpawnTower(index, cell, cellCenter, objectByType[cell[index]][0], root);
                        break;

                    case LevelTypeObject.EnemyTower:
                        levelRoot.EnemyTower = 
                            SpawnTower(index, cell, cellCenter, objectByType[cell[index]][0], root);
                        break;

                    case LevelTypeObject.Road:
                        SpawnRoad(cellCenter, objectByType[cell[index]], root);
                        break;
                }

                index++;
            }
        }
    }

    private void SpawnPlane(GameObject plane, Transform root)
    {
        float totalWidth = _gridConfig.Col * _cellSize;
        float totalDepth = _gridConfig.Row * _cellSize;

        Vector3 position = new Vector3(_levelCenter.x, 0f, _levelCenter.z);

        GameObject planeObject = Instantiate(plane, position, Quaternion.identity, root);
    
        Renderer renderer = planeObject.GetComponent<Renderer>();

        Vector3 localSize = planeObject.transform.InverseTransformVector(renderer.bounds.size);

        float scaleX = totalWidth / localSize.x;
        float scaleZ = totalDepth / localSize.z;

        NavMeshModifier meshModifier = planeObject.AddComponent<NavMeshModifier>();
        meshModifier.applyToChildren = false;
        meshModifier.overrideArea = true;
        meshModifier.area = 1;

        planeObject.transform.localPosition = new Vector3(
            planeObject.transform.localPosition.x,
            planeObject.transform.localPosition.y - localSize.y,
            planeObject.transform.localPosition.z
            );

        planeObject.transform.localScale = new Vector3(scaleX, localSize.y, scaleZ);
    }

    private void SpawnProps(Vector3 cellCenter, List<GameObject> props, Transform root)
    {
        int count = UnityEngine.Random.Range(_propsPerCellMin, _propsPerCellMax + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = props[UnityEngine.Random.Range(0, props.Count)];

            if (prefab == null) continue;

            Vector2 offset = UnityEngine.Random.insideUnitCircle * propsScatterRadius;
            Vector3 position = cellCenter + new Vector3(offset.x, 0f, offset.y);
        
            Instantiate(prefab, position, Quaternion.identity, root);
        }
    }

    private void SpawnRoad(Vector3 cellCenter, List<GameObject> road, Transform root)
    {
        GameObject roadObject = Instantiate(road[0], cellCenter, Quaternion.identity, root);

        Renderer renderer = roadObject.GetComponent<Renderer>();

        Vector3 localSize = roadObject.transform.InverseTransformVector(renderer.bounds.size);
        
        float scaleX = _cellSize / localSize.x;
        float scaleZ = _cellSize / localSize.z;

        NavMeshModifier meshModifier = roadObject.AddComponent<NavMeshModifier>();
        meshModifier.applyToChildren = false;
        meshModifier.overrideArea = true;
        meshModifier.area = 0;

        roadObject.transform.localScale = new Vector3(scaleX, roadObject.transform.localScale.y, scaleZ);
    }

    private Transform SpawnTower(
        int index,
        List<LevelTypeObject> grid,
        Vector3 cellCenter, 
        GameObject tower, 
        Transform root)
    {
        bool roadOnLeft = grid[index - 1] == LevelTypeObject.Road;
        bool roadOnRight = grid[index + 1] == LevelTypeObject.Road;

        float halfCell = _cellSize / 2f;

        Vector3 towerPosition = cellCenter;
        Vector3 extents = tower.GetComponent<Renderer>().bounds.extents;

        Quaternion towerRotation = Quaternion.identity;

        if (roadOnLeft)
        {
            towerRotation.y = 180f;
            towerPosition.x -= halfCell - extents.x;
        }

        if (roadOnRight) 
        {
            towerPosition.x += halfCell - extents.x; 
        }

        Debug.Log($"Tower root {root}");

        GameObject towerObject = Instantiate(tower, towerPosition, towerRotation, root);
        _DIContainer?.Inject(towerObject.GetComponent<Tower>());

        return towerObject.transform;
    }

    private void SetTowers()
    {
        LevelRoot levelRoot = _levelsRoots[_currentLevelOrder];

        Transform enemyTower = levelRoot.EnemyTower;
        Transform playerTower = levelRoot.PlayerTower;
    
        _unitSpawner?.SetTowers(enemyTower, playerTower);
    }
}

[System.Serializable]
public struct LevelOptions
{
    public int Order;
    public LevelSpawnConfig Config;
}

[System.Serializable]
public class LevelRoot
{
    public int Order;
    public int ChildIndex;

    public Transform EnemyTower;
    public Transform PlayerTower;
}