using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpellController))]
public class ExplodingSpell : MonoBehaviour
{
    public Explosion explosion;
    [Tooltip("Does the object get destroyed when it explodes?")]
    public bool destroyWhenExplodes = true;
    [Tooltip("Does the object need to contain the center of the collider to explode?")]
    public bool explodeOnTouch = false;
    public float delayBetweenExplosions = 0.5f;

    private List<Collider2D> affectedObjects;
    private SpellController spell;
    private ChainSpell chainSpell;
    private List<Collider2D> explosionsOnCooldown;
    private SpellAutoPilot autoPilot;

    void Awake()
    {
        affectedObjects = new List<Collider2D>();
        spell = GetComponent<SpellController>();
        chainSpell = GetComponent<ChainSpell>();
        explosionsOnCooldown = new List<Collider2D>();
        autoPilot = GetComponent<SpellAutoPilot>();
    }

    void Start()
    {
        if (GetComponent<MovingSpell>())
            StartCoroutine(explodeAfterSeconds(10));    // Avoid that a moving spell wander for eternity
    }

    void FixedUpdate()
    {
        foreach (Collider2D collider in affectedObjects)
        {
            if (!collider)
                continue;
            
            GameObject other = collider.gameObject;
            if (spell.emitter && other == spell.emitter.gameObject)
                continue;

            if (explodeOnTouch || collider.bounds.Contains(transform.position))
            {
                explode(collider);
                return;
            }
        }
    }

    public void explode(Collider2D collider)
    {
        if (explosionsOnCooldown.Contains(collider))
            return;

        if (explosion != null)
        {
            Explosion newExplosion = Instantiate(explosion).GetComponent<Explosion>();
            if (destroyWhenExplodes)
                newExplosion.transform.position = transform.position;   // if the object explodes, the exposion is created where it was
            else
                newExplosion.transform.position = collider.transform.position; 

            newExplosion.initialize(spell);
        }
        Damageable dmg = collider.GetComponent<Damageable>();
        if (dmg && chainSpell)
            chainSpell.bounce(collider);
        if (destroyWhenExplodes)
            Destroy(gameObject);
        else
        {
            StartCoroutine(startExplosionCooldown(collider));
        }
    }

    public void explode()
    {
        if (explosion != null)
        {
            Explosion newExplosion = Instantiate(explosion).GetComponent<Explosion>();
            newExplosion.transform.position = transform.position;   // if the object explodes, the exposion is created where it was

            newExplosion.initialize(spell);
        }
        if (chainSpell)
            chainSpell.bounce(null);
        if (destroyWhenExplodes)
            Destroy(gameObject);
    }

    private IEnumerator startExplosionCooldown(Collider2D col)
    {
        explosionsOnCooldown.Add(col);
        yield return new WaitForSeconds(delayBetweenExplosions);
        explosionsOnCooldown.Remove(col);
    }

    // Weird fix because OnTriggerStay2D randomly doesn't work. Need to keep a list of objects triggering
    void OnTriggerEnter2D(Collider2D other)
    {
        if (spell.ignoredColliders.Contains(other))
            return;

        if (!spell.collidesWithWalls && other.gameObject.layer == LayerMask.NameToLayer("BlockingLayer"))
            return;

        if (spell.collidesWithSpells &&
            (other.gameObject.layer == LayerMask.NameToLayer("Spells") || other.gameObject.layer == LayerMask.NameToLayer("MonsterSpells")))
            return;

        if (other.CompareTag("NoExplosion")) // Dont explose when colliding sphere
            return;

        if (autoPilot && autoPilot.explodeOnlyOnTarget && autoPilot.targetObject &&
            autoPilot.targetObject.gameObject != other.gameObject)
            return;

        if (other)
        {
            affectedObjects.Add(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other)
        {
            affectedObjects.Remove(other);
        }
    }

    IEnumerator explodeAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        explode();
    }

    /*
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Collision with "+other.name);
        if (((1 << other.gameObject.layer) & spellBlockingLayerWithMonster) != 0)       // Collision with a wall or a monster
        {
            if (other.gameObject == emitter.gameObject)
                return;

            if (other.bounds.Contains(transform.position))
            {
                Debug.Log("exploding! "+other.name+" and "+emitter.gameObject.name);
                explode();
                return;
            }
        }
    }*/
}
