using UnityEngine;
using UnityEditor;
using TMPro.EditorUtilities;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WaveManager manager  = (WaveManager)target;

        if (GUILayout.Button("Wave Settings"))
        {
            if (manager.Waves == null)
            {
                Debug.LogError("None Config !");
                return;
            }

            WaveWindow.ShowWindow(manager.WaveConfig);
        }

        if (GUILayout.Button("Start Wave"))
        {
            Debug.Log("Press Start Wave");
            manager.SetWave();
        }
    }
}
