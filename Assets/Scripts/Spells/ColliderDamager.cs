using UnityEngine;
using System.Collections;

public class ColliderDamager : MonoBehaviour
{
    [Tooltip("Damage compared to the original explosion")]
    public float damageRatio = 0.1f;
    private int damage;
    private SpellCaster emitter;

    private SpellController sp;
    private Explosion exp;

    void Awake()
    {
        sp = GetComponentInParent<SpellController>();
        exp = GetComponentInParent<Explosion>();
    }

    void Start()
    {
        if (exp)
        {
            damage = Mathf.CeilToInt(exp.damage * damageRatio);
            emitter = exp.emitter;
            return;
        }
        else if (sp)
        {
            damage = Mathf.CeilToInt(sp.damage * damageRatio);
            emitter = sp.emitter;
            return;
        }
        Debug.Log("Error while initializing " + name);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Damageable dmg = other.GetComponent<Damageable>();
        if (dmg)
        {
            dmg.doDamage(emitter, damage);
        }
    }
}
