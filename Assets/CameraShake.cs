using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour 
{
    public float intensity;
    public float duration;

	void Start () 
	{
        GameManager.instance.camera.shakeCamera(intensity, duration);
	}
}
