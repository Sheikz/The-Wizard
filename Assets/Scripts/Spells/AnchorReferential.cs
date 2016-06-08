using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnchorReferential : MonoBehaviour 
{
    public float rotationSpeed;
    public Vector3 velocity;
    public Transform parent;

	void FixedUpdate()
    {
        if (transform.childCount == 0)
            Destroy(gameObject);

        
        transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
        if (parent)
            transform.position = parent.transform.position;
        else 
            transform.position += velocity * Time.fixedDeltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
