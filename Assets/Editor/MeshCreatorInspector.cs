using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(MeshCreator))]
public class MeshCreatorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        MeshCreator meshCreator = (MeshCreator)target;

        DrawDefaultInspector();
        if (meshCreator.meshType == MeshCreator.MeshType.Wall)
        {
            meshCreator.hasExteriorCornerLeft = EditorGUILayout.Toggle("Left Exterior Corner", meshCreator.hasExteriorCornerLeft);
            meshCreator.hasInteriorCornerLeft = EditorGUILayout.Toggle("Left Interior Corner", meshCreator.hasInteriorCornerLeft);
            meshCreator.hasExteriorCornerRight = EditorGUILayout.Toggle("Right Exterior Corner", meshCreator.hasExteriorCornerRight);
            meshCreator.hasInteriorCornerRight = EditorGUILayout.Toggle("Right Interior Corner", meshCreator.hasInteriorCornerRight);
            meshCreator.blockingLow = EditorGUILayout.Toggle("Blocking Low", meshCreator.blockingLow);
        }

        if (meshCreator.meshType == MeshCreator.MeshType.Deco1 || meshCreator.meshType == MeshCreator.MeshType.Deco2)
        {
            meshCreator.decoType = (MeshCreator.DecoType)EditorGUILayout.EnumPopup("Deco Type", meshCreator.decoType);
        }

        if (GUILayout.Button("Regenerate"))
        {
            meshCreator.BuildMesh();
        }
    }
}
