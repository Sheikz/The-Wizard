using UnityEngine;
using System.Collections;

public class ElectricBall2 : MovingSpell
{
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
