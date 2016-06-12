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
    private bool positionFixed;
    private PlayerStats playerStats;

    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 10;
        rectTransform = GetComponent<RectTransform>();
        positionFixed = false;
        playerStats = GameManager.instance.hero.GetComponent<PlayerStats>();
    }

    private string parseDescription(SpellController spell, int damage)
    {
        string result = "";
        string name = spell.spellName;
        if (name == "")
            name = "#Undefined#";

        result += "<size=18>"+name+"</size>";

        if ((spell.manaCost >= 0) && spell.manaCostInterval == 1f)
            result += "\nCost: <color=magenta>" + spell.manaCost + "</color> mana";
        else if ((spell.manaCost >= 0) && spell.manaCostInterval != 1f)
            result += "\nCost: <color=magenta>" + Mathf.RoundToInt(spell.manaCost/spell.manaCostInterval) + "</color> mana/sec";
        else
            result += "\nBuild up <color=magenta>" + (-spell.manaCost) + "</color> mana per hit";

        if (spell.castTime <= 0)
            result += "\nInstant cast";
        else
            result += "\nCast time: <color=lime>" + spell.castTime + "</color> sec";

        if (spell.cooldown > 0)
            result += "\nCooldown: <color=orange>" + spell.cooldown +"</color> sec";
        if (spell.duration > 0)
            result += "\nDuration: <color=orange>" + spell.duration + "</color> sec";
        result += "\n" + spell.spellTypeDescription;
        if (damage > 0)
        {
            SpellDamager spellDamager = spell.GetComponent<SpellDamager>();
            Laser laserDamager = spell.GetComponent<Laser>();
            if (spellDamager && spellDamager.delayBetweenDamage > 0)
                damage = Mathf.RoundToInt(damage / spellDamager.delayBetweenDamage);
            else if (laserDamager && laserDamager.delayBetweenDamage >0)
                damage = Mathf.RoundToInt(damage / laserDamager.delayBetweenDamage);

            result += "\n" + spell.damageString.Replace("<dmg>", "<color=orange>" + damage + "</color>");
        }
        result += "\n"+spell.description;

        foreach (ItemPerk perk in Enum.GetValues(typeof(ItemPerk)))
        {
            if (playerStats.getItemPerk(perk) && perk.getSpellName() == spell.spellName)
            {
                result += "\n<color=orange>• " + perk.getDescription() + "</color>";
            }
        }

        SlowTime slowTime = spell.GetComponent<SlowTime>();
        if (slowTime)
            result = result.Replace("<timeMultiplier>", "<color=magenta>"+((1-slowTime.timeMultiplier)*100).ToString() + "%</color>");

        BuffArea buffArea = spell.GetComponent<BuffArea>();
        if (buffArea && buffArea.buff.damageReduction > 0)
            result = result.Replace("<damagereduction>", "<color=orange>" + buffArea.buff.damageReduction * 100 + "%</color>");
        
        ExplodingSpell explodingSpell = spell.GetComponent<ExplodingSpell>();
        if (explodingSpell && explodingSpell.delayedExplosions.Length > 0)
            result = result.Replace("<delay>", "<color=magenta>" + explodingSpell.delayedExplosions[0] + "</color>");

        result = result.Replace("<n>", "\n");   // Put line feeds
        return result;
    }

    internal void refresh(string text)
    {
        tooltipText.text = text;
        if (gameObject.activeSelf)
            StartCoroutine(fixPosition());
    }

    internal void refresh(Buff buff)
    {
        tooltipText.text = parseBuffDescription(buff);
        if (gameObject.activeSelf)
            StartCoroutine(fixPosition());
    }

    private string parseBuffDescription(Buff buff)
    {
        string result = "";
        result += "<size=18>" + buff.name + "</size>";
        result += "\n" + buff.description;
        result = result.Replace("<n>", "\n");
        return result;
    }

    internal void refresh(InventoryStats stats, int value)
    {
        switch (stats)
        {
            case InventoryStats.Power:
                tooltipText.text = "Increase Magic Damage by <color=magenta>" + value / ItemManager.instance.powerToDamage *100f  + "%</color>";
                break;
        }
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
        string nameColor = "white";
        switch (itemStats.rarity)
        {
            case ItemRarity.Common: nameColor = "white"; break;
            case ItemRarity.Rare: nameColor = "blue"; break;
            case ItemRarity.Epic: nameColor = "magenta"; break;
            case ItemRarity.Legendary: nameColor = "orange"; break;
        }
        result += "<size=12>"+itemStats.rarity.ToString()+"</size>";
		result += "\n<size=18><color="+nameColor+">" + itemStats.name + "</color></size>";
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
        if (itemStats.criticalStrikeChance > 0)
            result += "\nCritical strike: +<color=orange>" + itemStats.criticalStrikeChance + "%</color>";
        if (itemStats.energyRegen > 0)
            result += "\nEnergy regen: +<color=magenta>" + itemStats.energyRegen + "</color> mana per second";
        if (itemStats.hp > 0)
		    result += "\nHP: +<color=green>" + itemStats.hp + "</color>";

        if (itemStats.moveSpeed > 0)
		    result += "\nMove speed: +<color=yellow>" + itemStats.moveSpeed + "</color>";

        // Item Perk List
        if (itemStats.itemPerks.Count > 0)
            result += "\n";
        foreach (ItemPerk perk in itemStats.itemPerks)
        {
            result += "\n<color=orange>• " + perk.getDescription()+"</color>";
        }

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
        if (positionFixed)
            yield break;

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
        positionFixed = true;
    }
}
