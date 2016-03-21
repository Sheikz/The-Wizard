using UnityEngine;
using System.Collections;

public class ElectricBall : MovingSpell 
{	
	public float fadeTime;
	public float oscillatingAmplitude;
	public float oscillatingFrequency;
    public GameObject explosion;

	private Vector2 lateralDirection;
    private float creationTime;

	new void Start()
	{
        base.Start();
		lateralDirection = Vector3.Cross(Vector3.back, rb.velocity).normalized;
        lateralDirection *= Random.Range(-1f, 1f);
        lateralDirection.Normalize();
        creationTime = Time.time;
	}

	public void FixedUpdate()
	{
        rb.AddForce(lateralDirection * Mathf.Cos((Time.time - creationTime) * oscillatingFrequency) * oscillatingAmplitude);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)lateralDirection);
    }
	
	void OnTriggerEnter2D(Collider2D collider)
	{
		Damageable dmg = collider.gameObject.GetComponent<Damageable>();
		if (dmg != null)
		{
			dmg.doDamage(emitter, damage);
            Debug.Log("exploding here 1 with " + collider.name);
            explode(true);
        }
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if ((((1 << other.gameObject.layer) & blockingLayer) != 0) && other.bounds.Contains(transform.position))
		{
            rb.velocity = Vector2.zero;
            Debug.Log("exploding here 2 with " + other.name);
            explode(true);
            if (!explosion)
                StartCoroutine(destroyAfterSeconds(fadeTime));
        }
	}

    private void explode(bool destroyed)
    {
        if (!explosion)
            return;
        GameObject newExplosion = Instantiate(explosion) as GameObject;
        newExplosion.transform.position = transform.position;
        newExplosion.GetComponent<Explosion>().initialize(this);
        newExplosion.layer = gameObject.layer;
        if (destroyed)
            Destroy(gameObject);
    }
}
