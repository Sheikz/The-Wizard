using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WallCreator))]
public class WallCreatorInspector : Editor
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