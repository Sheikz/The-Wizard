using UnityEngine;
using System.Collections;

public class ParticleDamager : MonoBehaviour
{
    private int damage;
    private SpellCaster emitter;

    void Start()
    {
        damage = GetComponentInParent<SpellController>().damage;
        emitter = GetComponentInParent<SpellController>().emitter;
    }

    void OnParticleCollision(GameObject other)
    {
        Damageable dmg = other.GetComponent<Damageable>();
        if (dmg)
        {
            dmg.doDamage(emitter, damage);
        }
    }
}
