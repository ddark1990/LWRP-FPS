using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnGameObjects))]
public class SpawnGameObjectsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var serialObj = (SpawnGameObjects)target; //make this object a serializedObject

        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        var spawnObjectsButton = GUILayout.Button("Spawn Objects");
        var clearPlacedObjects = GUILayout.Button("Clear Placed Objects");
        EditorGUILayout.EndHorizontal();

        if (spawnObjectsButton) serialObj.PlaceRandomObjects();
        if (clearPlacedObjects) serialObj.RemovePlacedObjects();
    }
}
