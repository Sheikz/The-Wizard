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

    void Awake()
    {
        affectedObjects = new List<Collider2D>();
        spell = GetComponent<SpellController>();
        chainSpell = GetComponent<ChainSpell>();
        explosionsOnCooldown = new List<Collider2D>();
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

            if (explodeOnTouch)
            {
                explode(collider);
                return;
            }
            else if (collider.bounds.Contains(transform.position))
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

            newExplosion.initialize(spell.emitter, spell.damage, spell.lightIntensity);
            newExplosion.gameObject.layer = gameObject.layer;
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
