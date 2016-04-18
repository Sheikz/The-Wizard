using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Laser : MonoBehaviour 
{
    public Explosion explosion;
    public float delayBetweenExplosions = 0.5f;
    private LineRenderer lineRenderer;

    private Vector2 offset = new Vector2(0, 0);
    private RaycastHit2D hit;
    private Collider2D lastCollider;
    private SpellController spell;

    private bool onExplosionCooldown = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector2 scale = new Vector2(0.2f, 1);
        lineRenderer.sharedMaterial.SetTextureScale("_MainTex", scale);
        lineRenderer.sortingLayerName = "BelowSpells";
        spell = GetComponent<SpellController>();
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
        if (!hit)
            return;

        if (hit.collider != lastCollider)
            explode();
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
            newExplosion.transform.position = hit.point;   // if the object explodes, the exposion is created where it was
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
        hit = Physics2D.Raycast(position, targetPosition-position, Mathf.Infinity, GameManager.instance.layerManager.blockingLayer);
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, hit.point);

        RaycastHit2D[] hits = Physics2D.LinecastAll(position, hit.point, GameManager.instance.layerManager.monsterLayer);
        foreach (RaycastHit2D h in hits)
        {
            Damageable dmg = h.collider.GetComponent<Damageable>();
            if (dmg)
            {
                dmg.inflictDamage(spell.emitter, spell.damage);
            }
        }
    }
}

