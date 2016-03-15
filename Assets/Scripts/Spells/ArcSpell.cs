using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System;

public class ArcSpell : SpellController 
{
    public SpellController spell;
    public int nbrSpellsPerSide;
    public float angle;
    public Count angleOffset;

    private Vector3 position;
    private Vector3 target;
    private float offset;

    private void createSpells(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        Vector3 newTarget;
        Vector3 direction = target - position;
        Quaternion rotateQuat;
        offset = Random.Range(angleOffset.minimum, angleOffset.maximum);

        rotateQuat = Quaternion.Euler(0, 0, offset);
        newTarget = position + rotateQuat * direction;
        spell.castSpell(emitter, position, newTarget);

        for (int i=1; i <= nbrSpellsPerSide; i++)
        {
            rotateQuat = Quaternion.Euler(0, 0, i* angle + offset);
            newTarget = position + rotateQuat*direction;
            spell.castSpell(emitter, position, newTarget);

            rotateQuat = Quaternion.Euler(0, 0, -i* angle + offset);
            newTarget = position + rotateQuat*direction;;
            spell.castSpell(emitter, position, newTarget);
        }
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        createSpells(emitter, position, target);
        return this;
    }
}
