using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomLiaison : MonoBehaviour 
{
	void Start () 
	{
        foreach (MeshCreator meshCreator in GetComponentsInChildren<MeshCreator>())
        {
            meshCreator.BuildMesh();
        }
	}
}
