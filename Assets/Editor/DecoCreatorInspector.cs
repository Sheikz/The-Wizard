using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DecoCreator))]
public class DecoCreatorInspector : Editor
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