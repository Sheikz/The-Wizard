using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class Damager : MonoBehaviour
{
    public enum DamageType { DamageOnce, DamageOverTime, None };
    public DamageType damageType = DamageType.DamageOnce;
    protected List<Damageable> damagedObjects;
    protected Collider2D col;

    public float delayBetweenDamage = 0.25f;

    protected void Awake()
    {
        damagedObjects = new List<Damageable>();
        col = GetComponent<Collider2D>();
    }

    protected IEnumerator damageObject(Damageable dmg)
    {
        damagedObjects.Add(dmg);
        if (damageType == DamageType.DamageOnce)
            yield break;

        yield return new WaitForSeconds(delayBetweenDamage);
        damagedObjects.Remove(dmg);
    }
}