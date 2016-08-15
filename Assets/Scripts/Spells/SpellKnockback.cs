using UnityEngine;
using System.Collections.Generic;

public class SpellKnockback : MonoBehaviour
{
    public enum KnockbackDirection { FromCenter, ParallelToVelocity };

    public KnockbackDirection knockbackDirection = KnockbackDirection.ParallelToVelocity;
    public float knockbackForce = 100;
    public float stunDuration = 0.5f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        Knockbackable kb = collision.GetComponent<Knockbackable>();
        if (kb)
            applyKnockback(kb);
    }

    void applyKnockback(Knockbackable kb)
    {
        switch(knockbackDirection)
        {
            case KnockbackDirection.ParallelToVelocity:
                kb.knockback(rb.velocity * knockbackForce, stunDuration);
                break;
            case KnockbackDirection.FromCenter:
                Vector2 force = (kb.transform.position - transform.position).normalized * knockbackForce;
                kb.knockback(force, stunDuration);
                break;
        }
    }


}
