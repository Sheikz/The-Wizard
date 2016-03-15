using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FloorCreator))]
public class FloorCreatorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            (target as WorldObjectCreator).refreshContents();
        }
    }
}