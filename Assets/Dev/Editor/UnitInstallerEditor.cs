using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitInstaller))]
public class UnitInstallerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UnitInstaller unitInstaller = (UnitInstaller)target;

        if(GUILayout.Button("Initialized Unit"))
        {
            unitInstaller.InitializedUnit();
        }
    }
}
