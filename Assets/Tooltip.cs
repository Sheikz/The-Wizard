using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Tooltip : MonoBehaviour
{
    public Text tooltipText;

    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 10;
    }

    private string parseDescription(SpellController spell, int damage)
    {
        string result = "";
        string name = spell.spellName;
        if (name == "")
            name = "#Undefined#";

        result += "<size=18>"+name+"</size>" + "\n";
        if (spell.cooldown >0)
            result += "Cooldown: <color=orange>"+spell.cooldown +"</color> sec\n";
        if (spell.duration > 0)
            result += "Duration: <color=orange>" + spell.duration + "</color> sec\n";
        result += spell.spellTypeDescription + "\n";
        if (damage > 0)
            result += spell.damageString.Replace("<dmg>", "<color=orange>" + damage + "</color>") + "\n";
        result += spell.description;
        return result;
    }

    public void refresh(PlayerStats heroStats, SpellController containedSpell)
    {
        int damage = containedSpell.getDamage(heroStats);
        tooltipText.text = parseDescription(containedSpell, damage);

    }
}
