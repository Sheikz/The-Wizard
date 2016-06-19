using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Laser : MonoBehaviour 
{
    public Explosion explosion;
    public float delayBetweenExplosions = 0.5f;
    public float delayBetweenDamage = 0.25f;
    public float distance = 10f;
    private LineRenderer lineRenderer;

    private Vector2 offset = new Vector2(0, 0);
    private RaycastHit2D hit;
    private Collider2D lastCollider;
    private SpellController spell;
    private List<Damageable> damagedObjects;
    private StatusEffect[] statusEffects;
    private Vector3 hitPoint;

    private bool onExplosionCooldown = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector2 scale = new Vector2(0.2f, 1);
        lineRenderer.sharedMaterial.SetTextureScale("_MainTex", scale);
        lineRenderer.sortingLayerName = "BelowSpells";
        spell = GetComponent<SpellController>();
        damagedObjects = new List<Damageable>();
        statusEffects = GetComponentsInChildren<StatusEffect>();
    }

    void FixedUpdate()
    {
        offset.x += 0.03f;
        offset.x %= 1f;

        lineRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
        refreshExplosion();
    }

    void refreshExplosion()
    {
        if (hit && hit.collider != lastCollider)
            explode();
        if (hit && hit.collider)
            lastCollider = hit.collider;

        if (onExplosionCooldown)
            return;

        explode();
    }

    void explode()
    {
        if (explosion != null)
        {
            Explosion newExplosion = Instantiate(explosion);
            newExplosion.transform.position = hitPoint;   // if the object explodes, the exposion is created where it was
            newExplosion.initialize(spell);
        }
        StartCoroutine(startExplosionCooldown());
    }

    private IEnumerator startExplosionCooldown()
    {
        onExplosionCooldown = true;
        yield return new WaitForSeconds(delayBetweenExplosions);
        onExplosionCooldown = false;
    }

    internal void update(Vector3 position, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - position;
        direction.z = 0;
        hit = Physics2D.Raycast(position, direction, distance, GameManager.instance.layerManager.blockingLayer);
        lineRenderer.SetPosition(0, position);
        if (!hit)
            hitPoint = position + direction.normalized * distance;
        else
            hitPoint = hit.point;

        lineRenderer.SetPosition(1, hitPoint);

        RaycastHit2D[] hits = Physics2D.LinecastAll(position, hitPoint, GameManager.instance.layerManager.monsterLayer);
        foreach (RaycastHit2D h in hits)
        {
            Damageable dmg = h.collider.GetComponent<Damageable>();
            if (dmg)
                applyDamage(dmg);
        }
    }

    void applyDamage(Damageable dmg)
    {
        if (damagedObjects.Contains(dmg))
            return;

        dmg.doDamage(spell.emitter, spell.damage);
        StartCoroutine(damageObjectCooldown(dmg));

        StatusEffectReceiver receiver = dmg.GetComponent<StatusEffectReceiver>();
        if (!receiver)
            return;

        foreach (StatusEffect effect in statusEffects)
        {
            effect.inflictStatus(receiver);
        }
        
    }

    IEnumerator damageObjectCooldown(Damageable dmg)
    {
        damagedObjects.Add(dmg);
        yield return new WaitForSeconds(delayBetweenDamage);
        damagedObjects.Remove(dmg);
    }
}

