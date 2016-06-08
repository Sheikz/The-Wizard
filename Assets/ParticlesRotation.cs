using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ParticlesRotation : MonoBehaviour
{
    public Vector3 speed;

    [ExecuteInEditMode]
	void Update()
	{
        transform.Rotate(speed);
    }
}
