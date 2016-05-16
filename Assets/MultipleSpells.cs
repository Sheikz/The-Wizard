using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleSpells : MonoBehaviour 
{
    public int additionalSpellsPerSide;
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

        Vector3 newTarget;
        Vector3 direction = spell.target - transform.position;
        Quaternion rotateQuat;
        SpellController newSpell;
        MultipleSpells multi;

        for (int i = 1; i <= additionalSpellsPerSide; i++)
        {
            rotateQuat = Quaternion.Euler(0, 0, i * angleBetweenSpells);
            newTarget = transform.position + rotateQuat * direction;
            newSpell = spell.castSpell(spell.emitter, newTarget);
            multi = newSpell.GetComponent<MultipleSpells>();
            if (multi)
                multi.canBeMultiplied = false;

            rotateQuat = Quaternion.Euler(0, 0, -i * angleBetweenSpells);
            newTarget = transform.position + rotateQuat * direction; ;
            newSpell = spell.castSpell(spell.emitter, newTarget);
            multi = newSpell.GetComponent<MultipleSpells>();
            if (multi)
                multi.canBeMultiplied = false;

        }
    }
}
