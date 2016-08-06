using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    private const int maxSize = 100;
    private List<SpellController> spellPool;

    void Awake()
    {
        spellPool = new List<SpellController>();
    }

    public void addToPool(SpellController spell)
    {
        if (spellPool.Count >= maxSize)
        {
            Destroy(spell.gameObject);
            return;
        }

        spellPool.Add(spell);
    }
}
