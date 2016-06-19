using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class StopRigidbody : MonoBehaviour 
{
    public float stopAfter = 0;
    private Rigidbody2D rb;

	void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (stopAfter > 0)
            Invoke("stopRigidbody", stopAfter);
    }

    void stopRigidbody()
    {
        rb.velocity = Vector2.zero;
    }
}
