using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpellController))]
public class PullForceSpell : MonoBehaviour
{
    public float pullStrength;

    private StaticSpell spell;

    void Awake()
    {
        spell = GetComponent<StaticSpell>();
    }

    void FixedUpdate()
    {
        foreach (Damageable dmg in spell.affectedObjects)
        {
            if (!dmg)
                continue;

            if (spell.emitter == dmg.gameObject)  // If the emitter is the same as the receiver, there is no pull
                return;

            Rigidbody2D otherRB = dmg.GetComponent<Rigidbody2D>();
            if (otherRB)
                otherRB.AddForce((transform.position - dmg.transform.position) * pullStrength);
        }
    }
}
