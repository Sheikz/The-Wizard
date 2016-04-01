using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Tooltip : MonoBehaviour
{
    public Text tooltipText;
    public int horizontalPadding = 10;
    public int verticalPadding = 10;
    private RectTransform rectTransform;

    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 10;
        rectTransform = GetComponent<RectTransform>();
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

        if (spell.cooldown > 0)
            result += "Cooldown: <color=orange>"+spell.cooldown +"</color> sec\n";
        if (spell.duration > 0)
            result += "Duration: <color=orange>" + spell.duration + "</color> sec\n";
        result += spell.spellTypeDescription + "\n";
        if (damage > 0)
        {
            SpellDamager spellDamager = spell.GetComponent<SpellDamager>();
            if (spellDamager && spellDamager.delayBetweenDamage > 0)
                damage = Mathf.RoundToInt(damage / spellDamager.delayBetweenDamage);

            result += spell.damageString.Replace("<dmg>", "<color=orange>" + damage + "</color>") + "\n";
        }
        result += spell.description;

        result = result.Replace("<n>", "\n");
        return result;
    }

    private string parseSkillDescription(Skill containedSkill)
    {
        if (containedSkill)
            return containedSkill.getDescription();

        return null;
    }

	private string parseItemDescription(EquipableItemStats itemStats)
	{
        if (itemStats == null)
            return "";
        
		string result = "";
		result += "<size=18>" + itemStats.name + "</size>";
        if (itemStats.power > 0)
		    result += "\nPower: +<color=magenta>" + itemStats.power + "</color>";
        for (int i = 0; i < itemStats.magicModifiers.Length; i++)
        {
            if (itemStats.magicModifiers[i] != 1f)
            {
                result += "\n"+Enum.GetNames(typeof(MagicElement))[i];
                int modValue = Mathf.RoundToInt((itemStats.magicModifiers[i] - 1f) * 100f);
                result += ": +<color=magenta>" + modValue + "%</color>";
            }
        }
        if (itemStats.hp > 0)
		    result += "\nHP: +<color=green>" + itemStats.hp + "</color>";

        if (itemStats.moveSpeed > 0)
		    result += "\nMove speed: +<color=yellow>" + itemStats.moveSpeed + "</color>";

		return result;
	}
		
    /// <summary>
    /// Refresh with skill info
    /// </summary>
    /// <param name="containedSkill"></param>
    public void refresh(Skill containedSkill)
    {
        tooltipText.text = parseSkillDescription(containedSkill);
        if (gameObject.activeSelf) 
            StartCoroutine(fixPosition());
    }

    /// <summary>
    /// Refresh with spell info
    /// </summary>
    /// <param name="heroStats"></param>
    /// <param name="containedSpell"></param>
    public void refresh(SpellCaster hero, SpellController containedSpell)
    {
        int damage = containedSpell.getDamage(hero);
        tooltipText.text = parseDescription(containedSpell, damage);
        StartCoroutine(fixPosition());
    }

	public void refresh(EquipableItemStats itemStats)
	{
		tooltipText.text = parseItemDescription (itemStats);
        if (tooltipText.text == "")
            gameObject.SetActive(false);
        
		if (gameObject.activeSelf) 
			StartCoroutine(fixPosition());
	}


    IEnumerator fixPosition()
    {
        yield return new WaitForEndOfFrame();
        float widthExcess = (transform.position.x + rectTransform.rect.width) - Screen.width;
        if (widthExcess >= horizontalPadding)
        {
            transform.position += Vector3.left * (widthExcess + horizontalPadding);
        }
        float heightExcess = (transform.position.y + rectTransform.rect.height) - Screen.height;
        if (heightExcess >= verticalPadding)
        {
            transform.position += Vector3.up * (heightExcess + verticalPadding);
        }
    }
}
