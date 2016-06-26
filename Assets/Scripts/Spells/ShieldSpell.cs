using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class ShieldSpell : SpellController
{
    new void Start()
    {
        base.Start();
        if (duration > 0)
            StartCoroutine(destroyAfterSeconds(duration));
        else
            Destroy(gameObject);

        applyLayer();
    }

    new void applyLayer()
    {
        if (emitter == null)
        {
            Debug.Log("Emitter not defined for " + name);
            return;
        }
        if (emitter.isMonster)
        {
            if (collidesWithSpells)
                gameObject.layer = LayerMask.NameToLayer("MonsterShield");
            else
                gameObject.layer = LayerMask.NameToLayer("MonsterSpells");
            enemyLayer = GameManager.instance.layerManager.heroLayer;
        }
        else
        {
            if (collidesWithSpells)
                gameObject.layer = LayerMask.NameToLayer("HeroShield");
            else
                gameObject.layer = LayerMask.NameToLayer("Spells");
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
        }
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        ShieldSpell newSpell = Instantiate(this);
        newSpell.initialize(emitter, emitter.transform.position, target);
        return newSpell;
    }

    private bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        transform.position = position;
        this.emitter = emitter;
        transform.SetParent(emitter.transform);
        return true;
    }
}
