using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains all the possible spells in the game
/// </summary>
public class SpellManager : MonoBehaviour
{
    public static SpellManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        name = "SpellManager";
    }

    public List<GameObject> spellList;
    public GameObject[] auraPrefabs;
    public GameObject[] auraActivatePrefabs;
    public GameObject[] auraDisablePrefabs;
    public SpellWarning spellWarning;
    public Sprite[] elementIcons;
    public Sprite freezeDebuffIcon;
    public Color freezeColorMask;
    public Sprite slowDebuffIcon;
    public Sprite stunIcon;

    public static GameObject getSpell(MagicElement element, SpellType type, SpellSet set)
    {
        return instance.spellList.Find(elem =>
        {
            SpellController spell = elem.GetComponent<SpellController>();
            return (spell.magicElement == element && spell.spellType == type && spell.spellSet == set);
        });
    }

}