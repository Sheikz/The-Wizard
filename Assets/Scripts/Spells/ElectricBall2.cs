using UnityEngine;
using System.Collections;

public class ElectricBall2 : MovingSpell
{
    public float duration;

    new void Start()
    {
        base.Start();
        if (duration > 0)
            StartCoroutine(destroyAfterSeconds(duration));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Damageable dmg = other.gameObject.GetComponent<Damageable>();
        if (dmg != null)
        {
            dmg.doDamage(emitter, damage);
        }

        if (((1 << other.gameObject.layer) & blockingLayer) != 0)
        {
            bounce();
        }
    }

    void bounce()
    {
        rb.velocity = -rb.velocity;
    }
  
}
