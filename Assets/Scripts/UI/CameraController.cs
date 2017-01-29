using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour {

	public float cameraWheelSpeed;
	public float minCameraSize;
	public float maxCameraSize;

	private Camera cam;
	private GameObject hero;
	private const int cameraZPosition = -5;
	private Vector3 shakeDisplacement = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		cam = GetComponent<Camera>();
		hero = GameManager.instance.hero.gameObject;
	}
	
	// Update is called once per frame
	void Update () 
	{
		checkWheelInput();
		updateCameraPosition();
	}

	private void updateCameraPosition()
	{
		if (hero == null)
			return;
		Vector3 cameraPosition = hero.transform.position + shakeDisplacement;
		cameraPosition.z = cameraZPosition;
		transform.position = cameraPosition;
	}

	private void checkWheelInput()
	{
		float wheel = Input.GetAxis("Mouse ScrollWheel");
		cam.orthographicSize -= wheel* cameraWheelSpeed;

		if (cam.orthographicSize > maxCameraSize)
			cam.orthographicSize = maxCameraSize;
		if (cam.orthographicSize < minCameraSize)
			cam.orthographicSize = minCameraSize;

	}

	public void shakeCamera(float intensity, float duration)
	{
		StartCoroutine(shakeCameraRoutine(intensity, duration));
	}

	private IEnumerator shakeCameraRoutine(float intensity, float duration)
	{
		float durationLeft = duration;
		while (durationLeft > 0)
		{
			shakeDisplacement = Random.insideUnitCircle.toVector3() * intensity * (durationLeft / duration);
			durationLeft -= Time.deltaTime;
			yield return null;
		}
		shakeDisplacement = Vector3.zero;
	}
}
