using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Laser : DamageEmitter 
{
    public GameObject start;
    public SpellDamager explosion;
    public Explosion hitExplosion;
    public float delayBetweenDamage = 0.25f;
    public float distance = 10f;
    public LineRenderer lineTemplate;
    public float startDistanceOffset;
    public bool piercesEnemies = true;
    public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
    public float textureLengthScale = 3; //Length of the beam texture
    public int additionalRaysPerSide = 0;

    private LineRenderer[] lineRenderers;
    private Vector2 offset = new Vector2(0, 0);
    private RaycastHit2D hit;
    private Collider2D lastCollider;
    private SpellController spell;
    private List<Damageable> damagedObjects;
    private StatusEffect[] statusEffects;
    private Vector3 startPosition;
    private List<DamageListener> damageListeners;

    private SpellDamager[] explosionInstance;
    private GameObject[] startInstance;

    private bool onExplosionCooldown = false;
    private bool onExplosionStartCooldown = false;
    [HideInInspector]
    public LayerMask enemyLayer;

    void Awake()
    {
        spell = GetComponent<SpellController>();
        damagedObjects = new List<Damageable>();
        statusEffects = GetComponentsInChildren<StatusEffect>();
        damageListeners = new List<DamageListener>();
        foreach (DamageListener d in GetComponents<DamageListener>())
        {
            damageListeners.Add(d);
        }
    }

    public void init()
    {
        lineRenderers = new LineRenderer[1 + additionalRaysPerSide * 2];
        explosionInstance = new SpellDamager[1 + additionalRaysPerSide * 2];
        startInstance = new GameObject[1 + additionalRaysPerSide * 2];
        enemyLayer = spell.emitter.enemyLayer;
        for (int i = 0; i < 1 + additionalRaysPerSide * 2; i++)
        {
            lineRenderers[i] = Instantiate(lineTemplate);
            lineRenderers[i].sortingLayerName = "BelowSpells";
        }
    }

    internal void stop()
    {
        foreach(GameObject o in startInstance)
        {
            Destroy(o.gameObject);
        }
        foreach(SpellDamager d in explosionInstance)
        {
            Destroy(d.gameObject);
        }
        foreach (LineRenderer l in lineRenderers)
        {
            Destroy(l.gameObject);
        }
    }

    internal void update(Vector3 position, Vector3 targetPosition)
    {
        LayerMask mask = GameManager.instance.layerManager.blockingLayer;
        if (!piercesEnemies) // Should stop at monsters if it does not pierce
            mask = mask | enemyLayer;

        Vector3 straightDirection = targetPosition - position;
        straightDirection.z = 0;
        int sign = -1;
        float degrees = 0;

        for (int i= 0; i < lineRenderers.Length; i++)
        {
            if (sign == 1)
                degrees += 15;
            Vector3 direction = Quaternion.Euler(0, 0, sign * degrees) * straightDirection;
            sign *= -1;
            updateLaser(position, direction, mask, i);
        }
    }
    
    void updateLaser(Vector3 position, Vector3 direction, LayerMask mask, int i)
    {
        Vector3 hitPoint;
        hit = Physics2D.Raycast(position, direction, distance, mask);
        startPosition = position + (startDistanceOffset * direction.normalized);
        lineRenderers[i].SetPosition(0, startPosition);
        if (!hit)
            hitPoint = position + direction.normalized * distance;
        else
            hitPoint = hit.point;

        if (!explosionInstance[i])
        {
            explosionInstance[i] = Instantiate(explosion);
            explosionInstance[i].spell = spell;
        }
        if (!startInstance[i])
        {
            startInstance[i] = Instantiate(start);
        }

        lineRenderers[i].SetPosition(1, hitPoint);
        explosionInstance[i].transform.position = hitPoint;
        startInstance[i].transform.position = startPosition;

        float actualDistance = (hitPoint - startPosition).magnitude;
        lineRenderers[i].sharedMaterial.mainTextureScale = new Vector2(actualDistance / textureLengthScale, 1);
        lineRenderers[i].sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);

        RaycastHit2D[] hits = Physics2D.LinecastAll(position, hitPoint, enemyLayer);
        foreach (RaycastHit2D h in hits)
        {
            Damageable dmg = h.collider.GetComponent<Damageable>();
            if(dmg)
                applyDamage(dmg, h.point);
        }
    }

    void applyDamage(Damageable dmg, Vector3 hitPoint)
    {
        if (damagedObjects.Contains(dmg))
            return;

        dmg.doDamage(spell.emitter, spell.damage);
        StartCoroutine(damageObjectCooldown(dmg));
        if (hitExplosion)
            Instantiate(hitExplosion, hitPoint, Quaternion.identity);

        foreach (DamageListener l in damageListeners)
        {
            l.onDamage(spell.emitter, dmg, spell.damage);
        }

        BuffsReceiver receiver = dmg.GetComponent<BuffsReceiver>();
        if (!receiver)
            return;

        foreach (StatusEffect effect in statusEffects)
        {
            effect.applyBuff(receiver);
        }
        
    }

    IEnumerator damageObjectCooldown(Damageable dmg)
    {
        damagedObjects.Add(dmg);
        yield return new WaitForSeconds(delayBetweenDamage);
        damagedObjects.Remove(dmg);
    }
}

