using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    
    public float cameraWheelSpeed;
    public float minCameraSize;
    public float maxCameraSize;

    private Camera cam;
    private GameObject hero;

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
        Vector3 cameraPosition = hero.transform.position;
        cameraPosition.z = -5;
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
}
