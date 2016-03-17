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
    
    void OnTriggerStay2D(Collider2D collision)
    {
        Damageable dmg = collision.GetComponent<Damageable>();
        if (!dmg)
            return;

        if (spell.emitter == dmg.gameObject)  // If the emitter is the same as the receiver, there is no pull
            return;

        Rigidbody2D otherRB = dmg.GetComponent<Rigidbody2D>();
        if (otherRB)
        {
            Vector3 forceToCenter = (transform.position - dmg.transform.position).normalized * 0.5f;
            Vector3 rotationForce = Vector3.Cross(forceToCenter, Vector3.forward) * 0.8f;
            otherRB.AddForce((forceToCenter + rotationForce)*pullStrength);
        }
    }
}
