using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class LevelBuilder : MonoBehaviour, IInitialized
{
    [Inject] private SceneLoader _sceneLoader;
    [Inject] private AssetProvider _assetProvider;
    [Inject] private DiContainer _DIContainer;

    [Header("Main Settings")]
    [SerializeField] private NavMeshSurface _meshSurface;
    [SerializeField] private LevelGridConfig _gridConfig;
    [SerializeField] private Vector3 _levelCenter;
    [SerializeField] private List<LevelOptions> _levels;
    [SerializeField] private List<LevelRoot> _levelsRoots;

    [Header("Props Settings")]
    [SerializeField] private int _propsPerCellMin = 1;
    [SerializeField] private int _propsPerCellMax = 5;
    [SerializeField] private float _propsScatterRadius = 0.4f;
    [SerializeField] private int _maxAttemp = 15;

    [HideInInspector] public UnitSpawner UnitSpawner;
    [HideInInspector] public GameManager GameManager;

    private int _cellSize;
    private int _currentLevelOrder = int.MaxValue;

    private readonly struct PlacedProps
    {
        public readonly Vector3 Position;
        public readonly float Radius;

        public PlacedProps(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
    }

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

    private void OnDisable()
    {
        GameManager.onEnd -= ClearAll;
    }

    public void Initialized()
    {
        GameManager.onEnd += ClearAll;
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
    }

    public void ClearAll()
    {
        if (_levelsRoots.Count <= 0) return;

        _assetProvider.UnloadAllAssets().Forget();

        foreach (var levelRoot in _levelsRoots)
        {
            if (transform.GetChild(0).gameObject != null)
                DestroyImmediate(transform.GetChild(0).gameObject, false);
        }

        _currentLevelOrder = int.MaxValue;
        _levelsRoots.Clear();

        _sceneLoader.UnloadScene("GamePlayScene").Forget();
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

    public async UniTask SpawnLevel(int levelIndex)
    {
        LevelOptions level = _levels[levelIndex];

        GameObject levelRoot = new GameObject($"LevelRoot{level.Order}");
        levelRoot.transform.SetParent(transform, false);

        LevelRoot newLevelLinks = new LevelRoot
        {
            Order = level.Order,
            ChildIndex = level.Order
        };

        _levelsRoots.Add(newLevelLinks);

        await SpawnLevelObjects(level, newLevelLinks);

        _meshSurface.BuildNavMesh();

        SetActiveLevel(levelIndex);
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

            SpawnLevelObjects(levelOptions, newLevelLinks).Forget();

            //EditorUtility.SetDirty(this);
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

    private async UniTask SpawnLevelObjects(LevelOptions levelOptions, LevelRoot levelRoot)
    {
        var cell = _gridConfig.GetCellObjects();
        var objectByType = levelOptions.Config.TypeObjects;
        _cellSize = _gridConfig.CellSize;

        Transform root = transform;

        await SpawnPlane(objectByType[LevelTypeObject.Plane][0], root);

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
                        await SpawnProps(cellCenter, objectByType[cell[index]], root);
                        break;
                    
                    case LevelTypeObject.PlayerTower :
                        levelRoot.PlayerTower = 
                             await SpawnTower(index, cell, cellCenter, objectByType[cell[index]][0], root);
                        break;

                    case LevelTypeObject.EnemyTower:
                        levelRoot.EnemyTower = 
                            await SpawnTower(index, cell, cellCenter, objectByType[cell[index]][0], root);
                        break;

                    case LevelTypeObject.Road:
                        await SpawnRoad(cellCenter, objectByType[cell[index]], root);
                        break;
                }

                index++;
            }
        }
    }

    private async UniTask SpawnPlane(AssetReferenceGameObject reference, Transform root)
    {
        float totalWidth = _gridConfig.Col * _cellSize;
        float totalDepth = _gridConfig.Row * _cellSize;

        Vector3 position = new Vector3(_levelCenter.x, 0f, _levelCenter.z);

        GameObject planePrefab = await _assetProvider.Load<GameObject>(reference);

        GameObject planeObject = Instantiate(planePrefab, position, Quaternion.identity, root);
        planeObject.transform.SetParent(root);

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

    private async UniTask SpawnProps(Vector3 cellCenter, List<AssetReferenceGameObject> props, Transform root)
    {
        int count = UnityEngine.Random.Range(_propsPerCellMin, _propsPerCellMax + 1);

        List<PlacedProps> placedProps = new List<PlacedProps>(count);

        for (int i = 0; i < count; i++)
        {
            AssetReferenceGameObject reference = props[UnityEngine.Random.Range(0, props.Count)];

            if (reference == null) continue;

            GameObject propPrefab = await _assetProvider.Load<GameObject>(reference);

            Vector3 position = Vector3.zero;
            bool isPlaced = false;
            int attempts = 0;

            while ( attempts < _maxAttemp && isPlaced == false)
            {
                Vector2 offset = UnityEngine.Random.insideUnitCircle * _propsScatterRadius;
                Vector3 tempPosition = cellCenter + new Vector3(offset.x, 0f, offset.y);

                bool collision = false;

                foreach (var p in placedProps)
                {
                    float minDistance = GetPropRadius(propPrefab) + p.Radius;

                    if(Vector3.SqrMagnitude(tempPosition - p.Position) < minDistance * minDistance)
                    {
                        collision = true;
                        break;
                    }
                }
            
                if(collision == false)
                {
                    position = tempPosition;
                    isPlaced = true;
                }

                attempts++;
            }

            if (isPlaced == false)
                continue;

            placedProps.Add(new PlacedProps(position, GetPropRadius(propPrefab)));

            if(propPrefab != null) 
                Instantiate(propPrefab, position, Quaternion.identity, root);
        }
    }

    private float GetPropRadius(GameObject prefab)
    {
        var renderers = prefab.GetComponentsInChildren<Renderer>();
        if(renderers.Length == 0) return 0.5f;

        Bounds bounds = renderers[0].bounds;

        for (int i = 0; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return Mathf.Max(bounds.extents.x, bounds.extents.z);
    }

    private async UniTask SpawnRoad(Vector3 cellCenter, List<AssetReferenceGameObject> road, Transform root)
    {
        GameObject roadPrefab = await _assetProvider.Load<GameObject>(road[0]);
        GameObject roadObject = Instantiate(roadPrefab, cellCenter, Quaternion.identity, root);

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

    private async UniTask<Transform> SpawnTower(
        int index,
        List<LevelTypeObject> grid,
        Vector3 cellCenter, 
        AssetReferenceGameObject refrence, 
        Transform root)
    {
        bool roadOnLeft = grid[index - 1] == LevelTypeObject.Road;
        bool roadOnRight = grid[index + 1] == LevelTypeObject.Road;

        float halfCell = _cellSize / 2f;

        GameObject towerPrefab = await _assetProvider.Load<GameObject>(refrence);

        Vector3 towerPosition = cellCenter;
        Vector3 extents = towerPrefab.GetComponent<Renderer>().bounds.extents;

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

        GameObject towerObject = Instantiate(towerPrefab, towerPosition, towerRotation, root);

        towerObject.GetComponent<Tower>().GameManager = GameManager;
        _DIContainer?.Inject(towerObject.GetComponent<Tower>());

        return towerObject.transform;
    }

    private void SetTowers()
    {
        LevelRoot levelRoot = _levelsRoots[0];

        Transform enemyTower = levelRoot.EnemyTower;
        Transform playerTower = levelRoot.PlayerTower;
    
        UnitSpawner.SetTowers(enemyTower, playerTower);
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