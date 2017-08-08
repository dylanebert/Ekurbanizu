using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridData))]
public class GridDataEditor : Editor {

    public override void OnInspectorGUI() {
        GridData tar = (GridData)target;
        tar.goalCommutes = EditorGUILayout.IntSlider("Goal Commutes: ", tar.goalCommutes, 1, 1000);
        tar.roadCount = EditorGUILayout.IntSlider("Roads: ", tar.roadCount, 0, 99);
        EditorGUI.BeginChangeCheck();
        tar.mapSize = EditorGUILayout.Vector2Field("Map Size: ", tar.mapSize);
        if (EditorGUI.EndChangeCheck()) {
            tar.UpdateMapSize();
        }
        base.OnInspectorGUI();
        EditorUtility.SetDirty(tar);
    }
}
