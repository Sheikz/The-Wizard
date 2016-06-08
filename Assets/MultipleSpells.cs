using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleSpells : MonoBehaviour
{
    public int numberOfSpells;
    [Tooltip("Rotation speed in degrees")]
    public float rotationSpeed = 5f;
    public float radialVelocity = 5f;
    public bool attachedToCaster = false;

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

        if (!activated)
            return;

        referential = new GameObject().AddComponent<AnchorReferential>();
        referential.transform.position = spell.emitter.transform.position;
        referential.rotationSpeed = rotationSpeed;
        if (attachedToCaster)
            referential.parent = spell.emitter.transform;

        for (int i = 0; i < numberOfSpells; i++)
        {
            castSpell(i * 360f / numberOfSpells);
        }
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
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
