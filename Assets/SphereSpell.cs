using UnityEngine;
using System.Collections.Generic;

public class SphereSpell : MonoBehaviour 
{
    public const float particleSizeToRadius = 3.2f;
    public float radius = 1f;

	void Awake () 
	{
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            setSize(ps);
        }
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider)
            collider.radius *= radius / particleSizeToRadius;
    }

    void setSize(ParticleSystem ps)
    {
        ps.Stop();
        if (ps.shape.enabled)
        {
            ParticleSystem.ShapeModule shapeModule = ps.shape;
            shapeModule.radius *= radius / particleSizeToRadius;
        }
        ps.startSize *= radius / particleSizeToRadius;
        ps.Play();
    }
}
