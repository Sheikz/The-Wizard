using UnityEngine;
using System.Collections.Generic;
using System;

public class SpellWindowByType : MonoBehaviour
{
    public TypeGroup[] typeGroups;

    public SpellBookSpell spellIconPrefab;

    public void initialize()
    {
        gameObject.SetActive(true);
        addAllSpells();
        activatePanel(0);
    }

    void addAllSpells()
    {
        List<GameObject> spellList = GameManager.instance.spellManager.spellList;
        foreach (GameObject sp in spellList)
        {
            if (!sp)
                continue;

            SpellController spell = sp.GetComponent<SpellController>();
            if (!spell)
            {
                Debug.Log("Spell " + sp.name + " is not valid");
                continue;
            }

            SpellBookSpell newIcon = Instantiate(spellIconPrefab);
            newIcon.initialize(spell);
            newIcon.transform.SetParent(typeGroups[(int)spell.spellType].spells[(int)spell.magicElement]);
        }
    }

    public void activatePanel(int number)
    {
        foreach (TypeGroup typeGroup in typeGroups)
        {
            typeGroup.gameObject.SetActive(false);
        }
        typeGroups[number].gameObject.SetActive(true);
    }

    public void activatePanel(SpellType test)
    {
        foreach (TypeGroup typeGroup in typeGroups)
        {
            typeGroup.gameObject.SetActive(false);
        }
        typeGroups[(int)test].gameObject.SetActive(true);
    }

    internal void close()
    {
        gameObject.SetActive(false);
    }

    internal void open()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            GameManager.instance.setPause(false);
        }
        else
        {
            UIManager.instance.closeWindows();
            gameObject.SetActive(true);
            GameManager.instance.setPause(true);
        }
    }
}
