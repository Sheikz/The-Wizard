using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class OscillatingSpell : MonoBehaviour
{
    public float oscillatingAmplitude;
    public float oscillatingFrequency;

    private Vector2 lateralDirection;
    private float creationTime;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        lateralDirection = Vector3.Cross(Vector3.back, rb.velocity.normalized);
        lateralDirection *= Random.Range(-1f, 1f);
        lateralDirection.Normalize();
        creationTime = Time.time;
    }

    void FixedUpdate()
    {
        rb.AddForce((lateralDirection * Mathf.Cos((Time.time - creationTime) * oscillatingFrequency) * oscillatingAmplitude) * rb.velocity.sqrMagnitude / 25);
    }
}
