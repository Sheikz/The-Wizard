using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpellController))]
public class CollidesWithSpells : MonoBehaviour
{
    private SpellController spell;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }

	void Start ()
    {
        applyLayer();
	}

    void applyLayer()
    {
        if (spell.emitter == null)
        {
            Debug.Log("Emitter not defined for " + name);
            return;
        }
        if (spell.emitter.isMonster)
        {
            gameObject.layer = LayerManager.instance.monsterSpellCollidingWithSpellsInt;
        }
        else
        {
            gameObject.layer = LayerManager.instance.heroSpellCollidingWithSpellsInt;
        }
    }
}
