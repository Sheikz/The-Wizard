using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleSpells : MonoBehaviour
{
    public enum MultipleSpellType {Circle, Arc };
    public MultipleSpellType type;
    public int numberOfSpells;
    [Tooltip("Rotation speed in degrees")]
    public float rotationSpeed = 5f;
    public float radialVelocity = 5f;
    public bool attachedToCaster = false;
    public float angle;

    [HideInInspector]
    public bool canBeMultiplied = true;
    [HideInInspector]
    public Vector3 localDirection;
    public bool activated = false;
    private SpellController spell;
    private AnchorReferential referential;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        if (!canBeMultiplied)
            return;

        Debug.Log("multi start");
        if (!activated)
            return;

        referential = new GameObject().AddComponent<AnchorReferential>();
        referential.transform.position = spell.emitter.transform.position;
        referential.rotationSpeed = rotationSpeed;
        if (attachedToCaster)
            referential.parent = spell.emitter.transform;

        for (int i = 0; i < numberOfSpells; i++)
        {
            switch (type)
            {
                case MultipleSpellType.Circle:
                    castSpell(i * 360f / numberOfSpells);
                    break;
                case MultipleSpellType.Arc:
                    castSpell(-(i+1) * angle);
                    castSpell((i+1) * angle);
                    break;
            }

        }
        if (type == MultipleSpellType.Circle)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (type == MultipleSpellType.Circle)
            radialMovement();
    }

    void castSpell(float angle)
    {
        Vector3 newTarget;
        Vector3 direction = spell.target - transform.position;
        Quaternion rotateQuat;
        SpellController newSpell;
        MultipleSpells multi;

        rotateQuat = Quaternion.Euler(0, 0, angle);
        newTarget = transform.position + rotateQuat * direction;
        newSpell = spell.castSpell(spell.emitter, newTarget);
        multi = newSpell.GetComponent<MultipleSpells>();
        if (multi)
        {
            multi.canBeMultiplied = false;
            multi.localDirection = (newTarget - transform.position).normalized;
        }
        newSpell.transform.SetParent(referential.transform);
    }

    void radialMovement()
    {
        transform.localPosition += radialVelocity * Time.fixedDeltaTime * localDirection;
    }
}
