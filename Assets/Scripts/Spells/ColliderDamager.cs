using UnityEngine;
using System.Collections;

public class ColliderDamager : Damager
{
    [Tooltip("Damage compared to the original explosion")]
    public float damageRatioFromParent = 0.1f;
    public float lifeDamageRatio;
    public bool mustContainCenter = false;
    private int damage;
    private SpellCaster emitter;

    private SpellController sp;
    private Explosion exp;

    new void Awake()
    {
        base.Awake();
        sp = GetComponentInParent<SpellController>();
        exp = GetComponentInParent<Explosion>();
    }

    void Start()
    {
        if (exp)
        {
            damage = Mathf.CeilToInt(exp.damage * damageRatioFromParent);
            emitter = exp.emitter;
            return;
        }
        else if (sp)
        {
            damage = Mathf.CeilToInt(sp.damage * damageRatioFromParent);
            emitter = sp.emitter;
            return;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Damageable dmg = other.GetComponent<Damageable>();
        if (damagedObjects.Contains(dmg))
            return;

        if (dmg)
        {
            if (mustContainCenter && !col.bounds.Contains(dmg.transform.position))
                return;
            if (exp || sp)
                dmg.doDamage(emitter, damage);
            else
                dmg.doDamageRatio(this, lifeDamageRatio);
        }
        StartCoroutine(damageObject(dmg));
    }
}
