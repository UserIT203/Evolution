using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.STP;

public class LevelBuilderWindow : EditorWindow
{
    private const string LEVEL_CONFIG_PATH = "Assets/Dev/Config/LevelBuilder";
    private const string GRID_CONFIG_PATH = "Assets/Dev/Config/LevelBuilder/LevelGridConfig.asset";
 
    private const float CELL_MULTIPLE = 10;
    private const int CELL_MAX_SIZE = 10;
    private const int CELL_MIN_SIZE = 1;
    private const float SPACING = 5;
    private const float SCROLL_VIEW_HEIGHT = 50f;

    private GUIStyle _cellButtonStyle;
    private string _configName = string.Empty;

    private Color _selectedColor = Color.white;

    private LevelGridConfig _gridConfig;
    private LevelSpawnConfig _selectedConfig;

    private Vector2 _scrollPosition;

    private bool _showMapFoldout = false;

    private Dictionary<LevelTypeObject, Vector2> _objectScrollPositions = new();
    private Dictionary<LevelTypeObject, bool> _foldoutStates = new();

    private Dictionary<LevelTypeObject, List<GameObject>> _typeObjects;
    
    [MenuItem("Window/LevelBuilder")]
    public static void ShowWindow()
    {
        GetWindow<LevelBuilderWindow>("Grid Editor");
    }

    public static void OpenWithConfig(LevelSpawnConfig config)
    {
        LevelBuilderWindow window = GetWindow<LevelBuilderWindow>("Grid Editor");
        window._selectedConfig = config;
        window.LoadFromConfig(config);
        window.Focus();
    }

    private void OnEnable()
    {
        Initialized();
    }

    private void OnDisable()
    {
        Debug.Log("Close Level Build Window");

        if (_gridConfig == null) return;

        EditorUtility.SetDirty(_gridConfig);
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        #region View Cell Type
        EditorGUILayout.LabelField("Object Color", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        foreach (var kvp in _gridConfig.TypeColors)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            GUILayout.Label(kvp.ObjectType.ToString());

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = {background = MakeTexture(4, 4, kvp.ColorCell)},
                fixedHeight = 25,
                fixedWidth = 25,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            if (GUILayout.Button(GUIContent.none, buttonStyle))
            {
                _selectedColor = kvp.ColorCell;
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(2f);

        EditorGUILayout.BeginHorizontal();
        
        int newRow = EditorGUILayout.IntField("Row", _gridConfig.Row);
        int newColumns = EditorGUILayout.IntField("Columns", _gridConfig.Col);

        newRow = Mathf.Max(1, newRow);
        newColumns = Mathf.Max(1, newColumns);

        if(newRow != _gridConfig.Row || newColumns != _gridConfig.Col)
        {
            _gridConfig.Row = newRow;
            _gridConfig.Col = newColumns;
            _gridConfig.ReziseGrid();
        }

        EditorGUILayout.EndHorizontal();

        int newValue = EditorGUILayout.DelayedIntField("Cell Size", _gridConfig.CellSize);
        _gridConfig.CellSize = Mathf.Clamp(newValue, CELL_MIN_SIZE, CELL_MAX_SIZE);
        #endregion

        #region Create Map
        _showMapFoldout = EditorGUILayout.Foldout(_showMapFoldout, "Object Map", false);

        if (_showMapFoldout == true)
        {

            for (int row = 0; row < _gridConfig.Row; row++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                for (int col = 0; col < _gridConfig.Col; col++)
                {
                    int index = row * _gridConfig.Col + col;
                    Color currentColor = _gridConfig.GetColor(index);

                    _cellButtonStyle = new GUIStyle(GUI.skin.button)
                    {
                        normal = { background = MakeTexture(2, 2, currentColor) },
                        fixedWidth = _gridConfig.CellSize * CELL_MULTIPLE,
                        fixedHeight = _gridConfig.CellSize * CELL_MULTIPLE
                    };

                    if (GUILayout.Button(GUIContent.none, _cellButtonStyle))
                    {
                        _gridConfig.SetColor(index, _selectedColor);
                    }

                    if (col < _gridConfig.Col - 1)
                        GUILayout.Space(SPACING);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(SPACING);
            }
        }
        #endregion

        #region Select Config
        EditorGUILayout.LabelField("Level Config", EditorStyles.boldLabel);

        _selectedConfig = (LevelSpawnConfig)EditorGUILayout.ObjectField(
            "Current Config",
            _selectedConfig,
            typeof(LevelSpawnConfig),
            false
            );

        if (_selectedConfig != null)
        {
            if (GUILayout.Button("Apply Config"))
            {
                LoadFromConfig(_selectedConfig);
            }
        }
        #endregion

        #region Select Objects
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Objects to type", EditorStyles.boldLabel);

        foreach (var obj in _typeObjects)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField(obj.Key.ToString());

            GameObject newObject = (GameObject)EditorGUILayout.ObjectField(
                "Add object",
                null,
                typeof(GameObject),
                true
                );

            if (newObject != null)
            {
                if (_typeObjects[obj.Key].Contains(newObject) == false)
                    _typeObjects[obj.Key].Add(newObject);
            }

            if (_foldoutStates.ContainsKey(obj.Key) == false)
                _foldoutStates[obj.Key] = true;

            int objectCount = _typeObjects[obj.Key].Count;
            string foldoutText = $"Current objects ({objectCount})";

            _foldoutStates[obj.Key] = EditorGUILayout.Foldout(
                _foldoutStates[obj.Key],
                foldoutText,
                true
                );

            if (_foldoutStates[obj.Key])
            {

                if (_objectScrollPositions.ContainsKey(obj.Key) == false)
                    _objectScrollPositions[obj.Key] = Vector2.zero;

                if (_typeObjects[obj.Key].Count > 0)
                {
                    _objectScrollPositions[obj.Key] = EditorGUILayout.BeginScrollView(
                        _objectScrollPositions[obj.Key],
                        GUILayout.Height(SCROLL_VIEW_HEIGHT)
                        );

                    for (int i = _typeObjects[obj.Key].Count - 1; i >= 0; i--)
                    {
                        var item = _typeObjects[obj.Key][i];

                        EditorGUILayout.BeginHorizontal();

                        if (item != null)
                            EditorGUILayout.ObjectField(item, typeof(GameObject), true);
                        else
                            EditorGUILayout.LabelField("<null>");

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                            _typeObjects[obj.Key].RemoveAt(i);

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField("None Objects", EditorStyles.miniBoldLabel);
                }
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndVertical();

        #endregion

        #region CreateConfig

        EditorGUILayout.Space(SPACING);

        EditorGUILayout.LabelField("Creat Config", EditorStyles.boldLabel);

        if (_selectedConfig == null)
        {
            _configName = EditorGUILayout.DelayedTextField("Input Config name", _configName);

            if (string.IsNullOrEmpty(_configName))
            {
                EditorGUILayout.HelpBox("Fill Config Name", MessageType.Warning);
            }

            if (GUILayout.Button("Create Config"))
            {
                if (string.IsNullOrEmpty(_configName) == false)
                    CreateNewConfig(_configName);
            }
        }
        else
        {
            if(GUILayout.Button("Save Config"))
                SaveToConfig(_selectedConfig);
        }
        #endregion

        EditorGUILayout.EndScrollView();
    }

    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = color;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    private void Initialized()
    {
        LoadOrCreateGridConfig();

        _typeObjects = new Dictionary<LevelTypeObject, List<GameObject>> ();

        _typeObjects.Add(LevelTypeObject.Plane, new List<GameObject>());
        _typeObjects.Add(LevelTypeObject.PlayerTower, new List<GameObject>());
        _typeObjects.Add(LevelTypeObject.EnemyTower, new List<GameObject>());
        _typeObjects.Add(LevelTypeObject.Props, new List<GameObject>());
        _typeObjects.Add(LevelTypeObject.Road, new List<GameObject>());
    }

    private void LoadOrCreateGridConfig()
    {
        _gridConfig = AssetDatabase.LoadAssetAtPath<LevelGridConfig>(GRID_CONFIG_PATH);

        if(_gridConfig == null)
        {
            _gridConfig = ScriptableObject.CreateInstance<LevelGridConfig>();

            AssetDatabase.CreateAsset(_gridConfig, GRID_CONFIG_PATH);
            AssetDatabase.SaveAssets();
        }
    }

    private void LoadFromConfig(LevelSpawnConfig config)
    {
        if (config == null) return;

        _typeObjects = config.TypeObjects;

        Repaint();
    }

    private void SaveToConfig(LevelSpawnConfig config)
    {
        if (config == null) return;

        config.SetData(_typeObjects);
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }

    private void CreateNewConfig(string fileName)
    {
        if (AssetDatabase.IsValidFolder(LEVEL_CONFIG_PATH) == false) return;

        string fullPath = $"{LEVEL_CONFIG_PATH}/{fileName}.asset";

        LevelSpawnConfig newConfig = ScriptableObject.CreateInstance<LevelSpawnConfig>();

        newConfig.SetData(_typeObjects);

        AssetDatabase.CreateAsset(newConfig, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.RevealInFinder(fullPath);
    }
}