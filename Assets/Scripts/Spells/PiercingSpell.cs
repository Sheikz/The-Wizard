using UnityEngine;
using System.Collections;

public class PiercingSpell : MovingSpell
{
    public GameObject explosion;
    public bool pierceCausesExplosion = false;
    public bool destroyIfFinished = false;

    private bool isOnExplosionCoolDown = false;
    private float timeBetweenExplosions = 0.5f;

    public override bool canCastSpell(SpellCaster spellCaster, Vector3 initialPos, Vector3 target)
    {
        if (spellCaster.spellList[(int)spellType].moveSpellCaster != null)    // Forbid casting a move spell caster while one is already alive
            return false;

        return base.canCastSpell(spellCaster, initialPos, target);
    }

    void FixedUpdate()
    {
        if (!destroyIfFinished)
            return;

        checkIfAlive();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Damageable dmg = other.gameObject.GetComponent<Damageable>();
        if (dmg != null)
        {
            if (pierceCausesExplosion && (emitter != other.gameObject) && !isOnExplosionCoolDown)
            {
                StartCoroutine(startExplosionCooldown(timeBetweenExplosions));
                explode(false);
            }
            else
                dmg.doDamage(emitter, damage);

        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if ((((1 << other.gameObject.layer) & blockingLayer) != 0) && other.bounds.Contains(transform.position))
        {
            rb.velocity = Vector2.zero;
            explode(true);
            if (!explosion)
                StartCoroutine(destroyAfterSeconds(0.5f));
        }
    }

    private void explode(bool destroyed)
    {
        if (!explosion)
            return;

        GameObject newExplosion = Instantiate(explosion) as GameObject;
        newExplosion.transform.position = transform.position;
        newExplosion.GetComponent<Explosion>().initialize(this);
        if (destroyed)
            Destroy(gameObject);
    }

    private IEnumerator startExplosionCooldown(float time)
    {
        isOnExplosionCoolDown = true;
        yield return new WaitForSeconds(time);
        isOnExplosionCoolDown = false;
    }
}
