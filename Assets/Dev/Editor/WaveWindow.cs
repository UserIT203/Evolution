using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


public class WaveWindow : EditorWindow
{
    private const string PATH_TO_SAVE_CONFIG = "Assets/Dev/EvolutionGame/Config/Levels";

    private int _currentWaveIndex = -1;
    private int _curentStageIndex = -1;

    private bool _isOpenConfigFoldout = false;
    private WavesConfig _dowloadConfig;


    private GUIStyle _stagesButtonStyle;
    private GUIStyle _waveSettingsLabel;

    private List<Wave> _waves;
    private WavesConfig _config;
    private string _configName;

    public static void ShowWindow(WavesConfig config)
    {
        WaveWindow window = GetWindow<WaveWindow>(true);

        window._config = config;
        window._waves = config.Waves;
    }

    [MenuItem("Window/Wave Builder")]
    public static void ShowWindow()
    {
        WaveWindow window = GetWindow<WaveWindow>(true);
        window._waves = new List<Wave>();
    }

    private void OnGUI()
    {
        SetElementStyle();

        ViewConfigSettings();

        GUILayout.BeginHorizontal();

        WavesView();

        WaveSettingView();

        StageSettingsView();

        GUILayout.EndHorizontal();
    }

    private void WavesView()
    {
        GUILayout.BeginVertical(GUILayout.Width(position.width * 0.2f));

        GUILayout.BeginScrollView(Vector3.zero, GUILayout.ExpandWidth(true));

        GUILayout.Label($"Waves", EditorStyles.boldLabel);

        if (_waves.Count > 0)
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                if(GUILayout.Button($"Wave {i + 1}"))
                {
                    _currentWaveIndex = i;
                    _curentStageIndex = -1;
                }
            }
        }
        else
        {
            GUILayout.Label("Create wave", EditorStyles.boldLabel);
        }

        if (GUILayout.Button("+ Add wave"))
        {
            _waves.Add(new Wave());
            _currentWaveIndex++;
        }

        GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }

    private void WaveSettingView()
    {
        GUILayout.BeginVertical(GUILayout.Width(position.width * 0.4f));

        GUILayout.BeginScrollView(Vector3.zero, GUILayout.ExpandWidth(true));

        if(_currentWaveIndex != -1)
        {
            GUILayout.Label($"Wave {_currentWaveIndex + 1}");

            _waves[_currentWaveIndex].WaveName 
                = EditorGUILayout.TextField("Wave name", _waves[_currentWaveIndex].WaveName, _waveSettingsLabel);
            _waves[_currentWaveIndex].Delay =
                EditorGUILayout.FloatField("Wave Delay", _waves[_currentWaveIndex].Delay, _waveSettingsLabel);

            GUILayout.Label("Stages:", EditorStyles.boldLabel);

            if(_waves[_currentWaveIndex].StagesList.Count > 0)
            {
                for (int i = 0; i < _waves[_currentWaveIndex].StagesList.Count; i++)
                {
                    if(i == _curentStageIndex)
                    {
                        GUI.color = Color.green;
                    }

                    if(GUILayout.Button($"Stage {i + 1}", _stagesButtonStyle))
                    {
                        _curentStageIndex = i;
                    }

                    GUI.color = Color.white;
                }
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+AddStage"))
            {
                _waves[_currentWaveIndex].StagesList.Add(new WaveStage());
            }

            if(GUILayout.Button("- RemoveStage"))
            {
                if(_curentStageIndex != -1)
                {
                    _waves[_currentWaveIndex].StagesList.RemoveAt(_curentStageIndex);
                    _curentStageIndex = _waves[_currentWaveIndex].StagesList.Count - 1;
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("- Remove wave"))
            {
                _waves.RemoveAt(_currentWaveIndex);
                _currentWaveIndex = _waves.Count - 1;
            }
        }
        else
        {
            GUILayout.Label($"Select wave", EditorStyles.boldLabel);
        }

        GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }

    private void StageSettingsView()
    {
        if (_curentStageIndex == -1) return;

        GUILayout.BeginVertical(GUILayout.Width(position.width * 0.4f));

        GUILayout.BeginScrollView(Vector3.zero, GUILayout.ExpandWidth(true));

        GUILayout.Label($"Stage {_curentStageIndex + 1}");

        if (_waves[_currentWaveIndex].StagesList.Count > 0)
        {
            WaveStage stageObject = _waves[_currentWaveIndex].StagesList[_curentStageIndex];

            EditorGUILayout.BeginVertical(GUI.skin.box);

            if(stageObject.StageUnits == null) stageObject.StageUnits = new List<UnitStage> { new UnitStage() };

            for (int i = 0; i < stageObject.StageUnits.Count; i++)
            {
                if (stageObject.StageUnits[i] == null) continue;

                stageObject.StageUnits[i].UnitType = (UnitType)EditorGUILayout.EnumPopup("Type", stageObject.StageUnits[i].UnitType);
                stageObject.StageUnits[i].Count = EditorGUILayout.IntField("Count", stageObject.StageUnits[i].Count);
            
                if(GUILayout.Button("X")) stageObject.StageUnits.Remove(stageObject.StageUnits[i]);
            }

            EditorGUILayout.Space(5f);

            if (GUILayout.Button("New Unit")) stageObject.StageUnits.Add(new UnitStage());

            EditorGUILayout.EndVertical();

            stageObject.Delay = EditorGUILayout.FloatField("Delay:", stageObject.Delay);
        }

        GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }

    private void SetElementStyle()
    {
        _stagesButtonStyle = new GUIStyle(GUI.skin.button);
        _stagesButtonStyle.fontSize = 12;

        _waveSettingsLabel = new GUIStyle(GUI.skin.label);
        _waveSettingsLabel.normal.background = EditorGUIUtility.whiteTexture;
        _waveSettingsLabel.normal.textColor = Color.black;
    }

    private void ViewConfigSettings()
    {
        EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);

        _isOpenConfigFoldout = EditorGUILayout.Foldout(
            _isOpenConfigFoldout,
            "Settings",
            true);

        EditorGUILayout.BeginVertical();

        if (_isOpenConfigFoldout == false) return;

        if (_dowloadConfig == null)
        {

            if (_config != null)
            {
                if (GUILayout.Button("Save")) _config.Waves = _waves;

                return;
            }

            _configName = EditorGUILayout.TextField("Config Name", _configName);

            if (string.IsNullOrEmpty(_configName))
            {
                EditorGUILayout.HelpBox("Fill Config Name", MessageType.Warning);
            }
            else
            {
                if (GUILayout.Button("Create Config"))
                {
                    CreateNewWaveConfig();
                }
            }

        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        _dowloadConfig = (WavesConfig)EditorGUILayout.ObjectField(
            "Add Config",
            _dowloadConfig,
            typeof(WavesConfig),
            true
            );

        if (_dowloadConfig != null)
        {
            if (GUILayout.Button("Select Config"))
            {
                _waves = _dowloadConfig.Waves;
            }

            if (GUILayout.Button("SaveConfig"))
            {
                _dowloadConfig.Waves = _waves;
                EditorUtility.SetDirty(_dowloadConfig);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void CreateNewWaveConfig()
    {
        if (AssetDatabase.IsValidFolder(PATH_TO_SAVE_CONFIG) == false) return;

        string fullPath = $"{PATH_TO_SAVE_CONFIG}/{_configName}.asset";

        WavesConfig newConfig = ScriptableObject.CreateInstance<WavesConfig>();
        newConfig.Waves = _waves;
        
        AssetDatabase.CreateAsset(newConfig, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
