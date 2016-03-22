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

        if ((spell.manaCost >= 0) && spell.manaCostInterval == 1f)
            result += "Cost: <color=magenta>" + spell.manaCost + "</color> mana\n";
        else if ((spell.manaCost >= 0) && spell.manaCostInterval != 1f)
            result += "Cost: <color=magenta>" + Mathf.RoundToInt(spell.manaCost/spell.manaCostInterval) + "</color> mana/sec\n";
        else
            result += "Build up <color=magenta>" + (-spell.manaCost) + "</color> mana per hit\n";

        if (spell.castTime <= 0)
            result += "Instant cast\n";
        else
            result += "Cast time: <color=lime>" + spell.castTime + "</color> sec\n";

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
