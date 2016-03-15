using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpellController))]
public class NovaSpell : MonoBehaviour
{
    private SpellController spell;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        transform.position = spell.emitter.transform.position;
        transform.SetParent(spell.emitter.transform);
    }
}
