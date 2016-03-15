using UnityEngine;
using System.Collections;

public class RotateSpell : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Vector2 direction = GetComponent<Rigidbody2D>().velocity;
        float angle = Vector3.Angle(Vector3.down, direction);
        if (direction.x <= 0)
            angle *= -1;
        transform.Rotate(new Vector3(0, 0, angle));
    }
}