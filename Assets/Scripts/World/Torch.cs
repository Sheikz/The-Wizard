using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour
{
	public GameObject torchLight;
	public LayerMask visionLayer;
	private Light torchLightComponent;
	private GameObject hero;
    private Vector3 lightPoint;       // The point from where the raycast start 

	// Use this for initialization
	void Start ()
	{
		GameObject newLight = Instantiate(torchLight, transform.position, Quaternion.identity) as GameObject;
		newLight.transform.SetParent(gameObject.transform);
		newLight.transform.localPosition = new Vector3(0, 0, -1);
		torchLightComponent = newLight.GetComponent<Light>();
		torchLightComponent.intensity = 0;
		hero = GameManager.instance.hero.gameObject;
	}

    public void initialize(Vector3 point)
    {
        lightPoint = point;
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (hero == null)
        {
            Debug.Log("Here");
            hero = GameManager.instance.hero.gameObject;
            return;
        }
        
		RaycastHit2D hit = Physics2D.Linecast(lightPoint, hero.transform.position, visionLayer);
		if (hit.collider.gameObject == hero)
			torchLightComponent.intensity = 1;
		else
			torchLightComponent.intensity = 0;
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(lightPoint, hero.transform.position);
    }
}
