using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelBuilder levelBuilder = target as LevelBuilder;

        if(GUILayout.Button("Rebuild Current Level"))
        {
            levelBuilder.RebuildSelectedLevel();
        }

        if(GUILayout.Button("Clear All Levels"))
        {
            levelBuilder.ClearAll();
        }

        EditorGUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Previous Level"))
        {
            levelBuilder.SetPreviousLevel();
        }

        if(levelBuilder.FocusLevelConfig != null)
        {
            if(GUILayout.Button($"Change {levelBuilder.FocusLevelConfig.name} config"))
            {
                LevelBuilderWindow.OpenWithConfig(levelBuilder.FocusLevelConfig);
            }
        }

        if (GUILayout.Button("Next Level"))
        {
            levelBuilder.SetNextLevel();
        }

        EditorGUILayout.EndHorizontal();
    }
}
