using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleSpells : MonoBehaviour 
{
    public int numberOfSpells;
    public float angleBetweenSpells;

    [HideInInspector]
    public bool canBeMultiplied = true;
    public bool activated = false;
    private SpellController spell;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        if (!canBeMultiplied)
            return;

        if (!activated)
            return;

        for (int i= 0; i < numberOfSpells; i++)
        {
            castSpell(i * 360f / numberOfSpells);
        }
    }

    void castSpell(float angle)
    {
        Vector3 newTarget;
        Vector3 direction = spell.target - transform.position;
        Quaternion rotateQuat;
        SpellController newSpell;
        MultipleSpells multi;
        MovingSpell movingSpell;

        rotateQuat = Quaternion.Euler(0, 0, angle);
        newTarget = transform.position + rotateQuat * direction; ;
        newSpell = spell.castSpell(spell.emitter, newTarget);
        multi = newSpell.GetComponent<MultipleSpells>();
        if (multi)
            multi.canBeMultiplied = false;
        movingSpell = newSpell.GetComponent<MovingSpell>();
        if (movingSpell)
            movingSpell.addLateralVelocity(0.1f);
    }
}
