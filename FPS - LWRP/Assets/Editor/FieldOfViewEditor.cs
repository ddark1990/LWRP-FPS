using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = new Color(0.16f, 1f, 0.47f);
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        // foreach (var visibleTargets in fow.visibleTargets)
        // {
        //     //Handles.DrawLine(fow.transform.position + new Vector3(0, 2.8f, 0), visibleTargets.position + new Vector3(0, 2.8f, 0));
        //     //Handles.DrawLine(fow.transform.position, visibleTargets.position);
        // }
    }
}
