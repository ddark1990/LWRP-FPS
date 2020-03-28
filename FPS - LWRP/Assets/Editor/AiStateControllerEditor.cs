using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AiController))]
public class AiStateControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var serialObj = (AiController)target; //getting all data from here

        var agroInfo = "Has Agro: " + serialObj.hasAgro;
        var targetInfo = serialObj.hasAgro ? " | Current Target: " + serialObj.target.name : " | Current Target: None ";
        var targetDistanceInfo = serialObj.hasAgro ? " | Distance Away From Target: " + (int)serialObj.distanceFromTarget + "m" : " | Distance Away From Target: 0m";

        var aiStateInfo = agroInfo + targetInfo + targetDistanceInfo;

        GUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Ai State", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(GUI.skin.box);
        //EditorGUILayout.LabelField(serialObj.currentAction ? "Current Action: " + serialObj.currentAction : "Current Action: Idle", EditorStyles.boldLabel, GUILayout.Width(225));
        EditorGUILayout.HelpBox(aiStateInfo, MessageType.Info, true);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        DrawDefaultInspector();
    }
}
