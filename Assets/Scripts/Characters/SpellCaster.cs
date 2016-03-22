using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;

public class SpellCaster : MonoBehaviour
{
    public bool useMana = false;
    public int maxMana;
    public float manaRegenPerSecond;
	public GameObject[] spellList;

    protected bool[] isOnCoolDown;
	private Image[] spellIcons;
    private HashSet<Spray> activeSprays;
    private bool isHero = false;
    private SpellBook spellBook;
    private bool isCasting = false;

    private CharacterStats characterStats;
    [HideInInspector]
    public bool isMonster;
    private List<Companion> companionList;
    private List<CompanionController> followerList;
    [HideInInspector]
    public List<CircleSpell> activeCircleSpells;
    private int maxNumberOfActiveCompanions = 3;
    private float mana;
    private CastingBar castingBar;
    private MovingCharacter movingCharacter;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        movingCharacter = GetComponent<MovingCharacter>();
    }

    // Use this for initialization
    void Start ()
	{
		isOnCoolDown = new bool[spellList.Length];
		resetCooldowns();
		spellIcons = new Image[spellList.Length];
        activeSprays = new HashSet<Spray>();
        companionList = new List<Companion>();
        followerList = new List<CompanionController>();
        mana = maxMana;

        if (tag == "Hero")
        {
            isMonster = false;
            isHero = true;
        }
        else if (tag == "HeroCompanion")
            isMonster = false;
        else
            isMonster = true;

        if (isHero)
        {
			setIcons();
            spellBook = UIManager.instance.spellBook;
            spellBook.addSpell(spellList);
        }
	}

    void FixedUpdate()
    {
        mana += manaRegenPerSecond * Time.fixedDeltaTime;
        if (mana >= maxMana)
            mana = maxMana;
    }

    public void levelUpFollowers()
    {
        foreach (CompanionController follower in followerList)
        {
            if (follower)
            {
                Debug.Log(follower.name + " levelup !");
                CharacterStats stats = follower.GetComponent<CharacterStats>();
                if (stats)
                    stats.levelUp();
            }
        }
    }

    /// <summary>
    /// Learn a new spell. Equip it and add it to the spellbook
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
	public bool addSpell(SpellController spell)
	{
        equipSpell(spell);
        spellBook.addSpell(spell);
        if (spell.removePrerequisites) // Need to remove the prerequisites from the book
        {
            foreach (GameObject prerequisite in spell.prerequisites)
            {
                spellBook.removeSpell(prerequisite.GetComponent<SpellController>());
            }
        } 
		return true;
	}

    public bool equipSpell(SpellController spell)
    {
        if (spellList[spell.spellType.getInt()] == spell.gameObject)
            return false;


        spellList[spell.spellType.getInt()] = spell.gameObject;
        refreshIcons();
        return true;
    }

	/// <summary>
	/// Start a cooldown
	/// </summary>
	/// <param name="spellIndex"></param>
	/// <returns></returns>
	private IEnumerator startCooldown(int spellIndex)
	{
		isOnCoolDown[spellIndex] = true;
        float cdModifier = characterStats ? characterStats.cooldownModifier : 1;
        float cooldown = spellList[spellIndex].GetComponent<SpellController>().cooldown * cdModifier;
		if (isHero)
		{
			float startingTime = Time.time;
			while (Time.time - startingTime < cooldown)
			{
                UIManager.instance.coolDownImages[spellIndex].fillAmount = Mathf.Lerp(1, 0, (Time.time - startingTime) / cooldown);
				yield return null;
			}
            UIManager.instance.coolDownImages[spellIndex].fillAmount = 0;

		}
		else
			yield return new WaitForSeconds(cooldown);
		isOnCoolDown[spellIndex] = false;
	}

	public void resetCooldowns()
	{
		for (int i = 0; i < isOnCoolDown.Length; i++)
		{
			isOnCoolDown[i] = false;
		}
	}

	public void castSpell(int spellIndex, Vector3 initialPos, Vector3 target)
	{
        if (isCasting)
            return;

        if (spellList.Length == 0)
            return;

		if (!spellList[spellIndex])
			return;
		if (!GameManager.instance.isPaused && !isOnCoolDown[spellIndex])
		{
            SpellController spell = spellList[spellIndex].GetComponent<SpellController>();
            if (useMana && mana < spell.manaCost)
                return;

            if (spell.canCastSpell(this, initialPos, target))
                StartCoroutine(castingSpellRoutine(spell, this, spellIndex, initialPos, target));
        }
	}

    /// <summary>
    /// Cast the spell if a casting time is specified. Then, cast the spell and start cooldown. Hide castbar after.
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="emitter"></param>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator castingSpellRoutine(SpellController spell, SpellCaster emitter, int index, Vector3 pos, Vector3 target)
    {
        if (spell.castTime > 0)
        {
            isCasting = true;
            float startingTime = Time.realtimeSinceStartup;
            if (movingCharacter)
                movingCharacter.enableMovement(false);
            while (Time.realtimeSinceStartup - startingTime < spell.castTime)
            {
                setCastBarRatio((Time.realtimeSinceStartup - startingTime) / spell.castTime);
                yield return null;
            }
            setCastBarRatio(1f);

            if (movingCharacter)
                movingCharacter.enableMovement(true);
        }

        if (spell.castSpell(this, pos, target))
        {
            StartCoroutine(startCooldown(index));  // Start the cooldown only if the spell has been launched
            payMana(spell.manaCost);
        }
        isCasting = false;
        if (!castingBar)
            yield break;

        yield return new WaitForSeconds(0.25f);
        castingBar.gameObject.SetActive(false);
    }

    private void setCastBarRatio(float ratio)
    {
        if (castingBar == null)
        {
            castingBar = Instantiate(UIManager.instance.castingBar);
            castingBar.transform.SetParent(transform);
            castingBar.transform.localPosition = new Vector3(0, -0.8f, 0);
        }
        castingBar.gameObject.SetActive(true);
        castingBar.setRatio(ratio);
    }

    private void payMana(float manaCost)
    {
        if (manaCost <= 0)
            return;

        mana -= manaCost;
        mana = Mathf.Clamp(mana, 0, maxMana);
    }

    internal void giveMana(float manaCost)
    {
        if (manaCost <= 0)
            return;

        mana += manaCost;
        mana = Mathf.Clamp(mana, 0, maxMana);
    }

    public void castAvailableSpells(Vector3 initialPosition, Vector3 target)
	{
		for (int i = 0; i < spellList.Length; i++)
		{
			castSpell(i, initialPosition, target);
		}
	}

    public GameObject[] getEquippedSpells()
    {
        return spellList;
    }

    public HashSet<SpellController> getKnownSpells()
    {
        return spellBook.getSpells();
    }

    /// <summary>
    /// Set the icons of the spells
    /// </summary>
    private void setIcons()
	{
		spellIcons[0] = GameObject.Find("SpellIconPrimarySpell").transform.FindChild("SpellIcon").GetComponent<Image>();
		spellIcons[1] = GameObject.Find("SpellIconSecondarySpell").transform.FindChild("SpellIcon").GetComponent<Image>();
		spellIcons[2] = GameObject.Find("SpellIconDefensive").transform.FindChild("SpellIcon").GetComponent<Image>();
		spellIcons[3] = GameObject.Find("SpellIconUltimate1").transform.FindChild("SpellIcon").GetComponent<Image>();
        spellIcons[4] = GameObject.Find("SpellIconUltimate2").transform.FindChild("SpellIcon").GetComponent<Image>();
        refreshIcons();
	}

	private void refreshIcons()
	{
		for (int i = 0; i < spellList.Length; i++)
		{
			if (!spellList[i])
			{
				Color c = spellIcons[i].color;
				c.a = 0f;
				spellIcons[i].color = c;
			}
			else
			{
				Color c = spellIcons[i].color;
				c.a = 1f;
				spellIcons[i].color = c;
				spellIcons[i].sprite = spellList[i].GetComponent<SpellController>().icon;
			}
		}
	}

    public void addSpray(Spray newSpray)
    {
        activeSprays.Add(newSpray);
    }

    public void removeSpray(Spray spray)
    {
        activeSprays.Remove(spray);
    }

    public Spray getActiveSpray(string name)
    {
        foreach (Spray sp in activeSprays)
        {
            if (sp == null)
            {
                activeSprays.Remove(sp);
                continue;
            }
            if (sp.name == name)
                return sp;
        }
        return null;
    }
        
    /// <summary>
    /// Check if we did not reach the max number of companions. If not, add it
    /// </summary>
    /// <param name="companion"></param>
    /// <returns></returns>
    public void addCompanion(Companion companion)
    {
        companionList.Add(companion);
        if (companionList.Count > maxNumberOfActiveCompanions)
        {
            Destroy(companionList[0].gameObject);
            companionList.RemoveAt(0);
        }
    }

    public void removeCompanion(Companion companion)
    {
        companionList.Remove(companion);
    }

    public void addFollower(CompanionController companion)
    {
        followerList.Add(companion);
    }

    public void removeFollower(CompanionController companion)
    {
        followerList.Remove(companion);
    }

    public float getMana()
    {
        return mana;
    }

    public float getManaRatio()
    {
        return mana / maxMana;
    }
}
