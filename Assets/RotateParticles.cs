using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RotateParticles : MonoBehaviour
{
    public Vector3 speed;

	void FixedUpdate()
	{
        transform.Rotate(speed);
    }
}
