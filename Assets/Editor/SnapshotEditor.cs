using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Snapshot))]
public class SnapshotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Assign the target object to the snapshot variable
        Snapshot snapshot = (Snapshot)target;
        
        // Draw the default inspector UI
        base.DrawDefaultInspector();
        
        // Add a button to reset entities
        if(GUILayout.Button("Reset Entities"))
        {
            snapshot.ResetEntities();
        }
    }
}