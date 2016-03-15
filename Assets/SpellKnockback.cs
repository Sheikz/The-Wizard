using UnityEngine;
using System.Collections.Generic;

public class SpellKnockback : MonoBehaviour
{
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
        kb.knockback(rb.velocity*100, 1);
    }


}
