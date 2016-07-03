using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SquareRoom))]
public class RoomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Refresh"))
        {
            SquareRoom room = (SquareRoom)target;
            room.refresh();
        }
    }
}