using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Door))]
public class DoorInspector : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck())
		{
			(target as Door).refresh();
		}
	}
}