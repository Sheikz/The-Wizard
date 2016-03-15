using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(MeshCreator))]
public class MeshCreatorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Regenerate"))
        {
            MeshCreator meshCreator = (MeshCreator)target;
            meshCreator.BuildMesh();
        }
    }
}
