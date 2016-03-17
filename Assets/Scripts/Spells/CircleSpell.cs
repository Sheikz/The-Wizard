using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class CircleSpell : SpellController
{
    private enum ShootType { AllDirection, Sequential, Together};
    public RotatingSpell spell;
    public int numberOfSpells;

    private List<RotatingSpell> spellCreated;
    private bool isCreatingSpell = true;
    private ShootType shootType = ShootType.AllDirection;

    new void Awake()
    {
        base.Awake();
        spellCreated = new List<RotatingSpell>();
        shootType = (ShootType)Random.Range(0, 3);
    }
   
    new void Start()
    {
        base.Start();
        transform.SetParent(emitter.transform);
        transform.position = emitter.transform.position;
        if (emitter.activeCircleSpells.Count > 0)
        {
            foreach (CircleSpell sp in emitter.activeCircleSpells)
            {
                sp.shootSpells();
                Destroy(sp.gameObject);
            }
            emitter.activeCircleSpells.Clear();
        }
        emitter.activeCircleSpells.Add(this);
        StartCoroutine(createSpells());
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        if (emitter.activeCircleSpells.Count > 0)
            return null;

        CircleSpell newSpell = Instantiate(this);
        newSpell.emitter = emitter;
        return newSpell;
    }

    void FixedUpdate()
    {
        if (!isCreatingSpell && transform.childCount == 0)
        {
            emitter.activeCircleSpells.Remove(this);
            Destroy(gameObject);
        }
    }

    private IEnumerator createSpells()
    {
        yield return new WaitForEndOfFrame();
        float timeBetweenSpells = 2 * Mathf.PI / numberOfSpells / spell.rotationSpeed;
        for (int i = 0; i < numberOfSpells; i++)
        {
            if (!isCreatingSpell)
                yield break;

            RotatingSpell newSpell = spell.castSpellWithReturn(emitter, emitter.transform.position, Vector3.zero, this);
            spellCreated.Add(newSpell);
            yield return new WaitForSeconds(timeBetweenSpells);
        }
        yield return new WaitForSeconds(0.1f);
        shootSpells();
    }

    private void shootSpells()
    {
        isCreatingSpell = false;
        foreach (RotatingSpell sp in spellCreated)
        {
            if (sp == null)
                continue;
            switch (shootType)
            {
                case ShootType.AllDirection: sp.shootImmediate();
                    break;
                case ShootType.Sequential: sp.shootSequential();
                    break;
                case ShootType.Together: sp.shootTogether(this);
                    break;
            }
        }
    }

    internal void shootInDirection(Vector3 direction)
    {
        foreach(RotatingSpell sp in spellCreated)
        {
            if (sp == null)
                continue;
            sp.shootInDirection(direction);
        }
    }
}
