using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class SpellCaster : MonoBehaviour
{
	public bool useMana = false;
	public int maxMana;
	public float baseManaRegen;
    public int baseCritChance;
	public SpellController[] spellList;

	protected bool[] isOnCoolDown;
	private Image[] spellIcons;
    private HoveringToolTip[] tooltips;
    [HideInInspector]
	public bool isHero = false;

    private SpellBook spellBook;

	private bool isCasting = false;
    private CharacterStats characterStats;
    [HideInInspector]
    public PlayerStats playerStats;
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
	private Animator anim;
	[HideInInspector]
	public Transform targetOpponent;
    [HideInInspector]
    public Transform targetAlly;
    private BuffsReceiver buffReceiver;
    private float itemManaRegen;
    private int itemCritChance;

    private List<PowerUpBuff> activeBuffs;
    private bool payChannelManaOnCooldown = false;

    void Awake()
	{
		characterStats = GetComponent<CharacterStats>();
		movingCharacter = GetComponent<MovingCharacter>();
		anim = GetComponent<Animator>();
        buffReceiver = GetComponent<BuffsReceiver>();
        playerStats = GetComponent<PlayerStats>();
    }

	// Use this for initialization
	void Start ()
	{
		isOnCoolDown = new bool[spellList.Length];
		resetCooldowns();
		spellIcons = new Image[spellList.Length];
        tooltips = new HoveringToolTip[spellList.Length];
		companionList = new List<Companion>();
		followerList = new List<CompanionController>();
		activeBuffs = new List<PowerUpBuff>();
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
        mana += getManaRegen() * Time.fixedDeltaTime;
		if (mana >= maxMana)
			mana = maxMana;
	}

    public float getManaRegen()
    {
        return baseManaRegen + itemManaRegen;
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
		return true;
	}

	public bool equipSpell(SpellController spell)
	{
		if (!spell)
			return false;

		if (spellList[spell.spellType.getInt()] == spell)
			return false;

		spellList[spell.spellType.getInt()] = spell;
		refreshIcons();
		return true;
	}

    /// <summary>
    /// Does this spellcaster have healing spells? (with negative damage)
    /// </summary>
    /// <returns></returns>
    internal bool hasSupportSpells()
    {
        foreach (SpellController spell in spellList)
        {
            if (spell.damage < 0)
                return true;
        }
        return false;
    }

    internal void startCooldown(SpellType spellType, float cooldown)
	{
		StartCoroutine(startCooldown((int)spellType, cooldown));
	}

	/// <summary>
	/// Start a cooldown
	/// </summary>
	/// <param name="spellIndex"></param>
	/// <returns></returns>
	private IEnumerator startCooldown(int spellIndex, float cooldown = 0)
	{
		isOnCoolDown[spellIndex] = true;
		float cdModifier = characterStats ? characterStats.cooldownModifier : 1;
		if (cooldown == 0)
		cooldown = spellList[spellIndex].GetComponent<SpellController>().getCooldown(this) * cdModifier;
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

	internal float getActiveBuff(MagicElement magicElement)
	{
		float result = 1.0f;
		foreach (PowerUpBuff buff in activeBuffs)
		{
			if (buff.element == magicElement)
			{
				result *= buff.multiplier;
			}
		}
		return result;
	}

	public void resetCooldowns()
	{
		for (int i = 0; i < isOnCoolDown.Length; i++)
		{
			isOnCoolDown[i] = false;
		}
	}

	public void castSpell(int spellIndex, Vector3 target)
	{
		if (GameManager.instance.isPaused || isOnCoolDown[spellIndex])
			return;

		if (spellList.Length == 0)
			return;

		if (movingCharacter && !movingCharacter.canAct)
			return;

        if (buffReceiver && buffReceiver.isStunned)
            return;

		if (!spellList[spellIndex])
			return;

		SpellController spell = spellList[spellIndex].GetComponent<SpellController>();

		if (isCasting)
			return;

        if (useMana && mana < spell.manaCost)
        {
            if (isHero)
                SoundManager.instance.playSound("NotEnoughMana");
            return;
        }

		if (spell.canCastSpell(this, transform.position, target))
		{
			ChargingSpell chargingSpell = spell.GetComponent<ChargingSpell>();
            if (chargingSpell)
            {
                StartCoroutine(chargingSpellRoutine(chargingSpell, spellIndex));
                return;
            }
            ChannelSpell channelSpell = spell.GetComponent<ChannelSpell>();
            if (channelSpell)
            {
                StartCoroutine(channelSpellRoutine(channelSpell, spellIndex));
                return;
            }

            StartCoroutine(castingSpellRoutine(spell, spellIndex, target));
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
    private IEnumerator castingSpellRoutine(SpellController spell, int index, Vector3 targetPosition)
	{
        SpellWarning spellWarn;
        if (anim)
            anim.SetInteger("AttackType", Random.Range(0, 2) + 1);  // Randomizing the attack type

        float castTime = spell.getCastTime(this);
		if (castTime > 0)
		{
            if (anim)
                anim.SetTrigger("PrepareAttack");
            if (spell.hasSpellWarning && isMonster)
            {
                spellWarn = Instantiate(SpellManager.instance.spellWarning, transform.position, Quaternion.identity) as SpellWarning;
                spellWarn.init(spell, this);
            }

			isCasting = true;
			float startingTime = Time.time;
			if (movingCharacter)
				movingCharacter.enableMovement(false);
			while (Time.time - startingTime < castTime)
			{
				if (buffReceiver && buffReceiver.isStunned)  // If the character is stunned while casting, it should stop the cast
				{
					isCasting = false;
					if (!castingBar)
						yield break;

					yield return new WaitForSeconds(0.25f);
					castingBar.gameObject.SetActive(false);
					yield break;
				}

				setCastBarRatio((Time.time - startingTime) / castTime);
                if (isHero)
                {
                    targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    targetPosition.z = 0;    // fix because camera see point at z = -5
                }
                updateFacingDirection(targetPosition);
                yield return new WaitForFixedUpdate();
			}
			setCastBarRatio(1f);

			if (movingCharacter)
				movingCharacter.enableMovement(true);

            if (spell.damage >= 0 && targetOpponent)
                targetPosition = targetOpponent.position;
            else if (spell.damage < 0 && targetAlly)
                targetPosition = targetAlly.position;
            else if (isHero)
            {
                targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = 0;    // fix because camera see point at z = -5
            }
		}

		if (spell.castSpell(this, targetPosition) && payMana(spell.manaCost))
		{
			StartCoroutine(startCooldown(index));  // Start the cooldown only if the spell has been launched
            if (anim)
                anim.SetTrigger("Attack");
            updateFacingDirection(targetPosition);
        }
		else
		{
            if (anim)
                anim.SetTrigger("CancelAttack");
		}
		isCasting = false;
		if (!castingBar)
			yield break;

		yield return new WaitForSeconds(0.25f);
		castingBar.gameObject.SetActive(false);
	}

    /// <summary>
    /// This will fail is casted by monsters as it's always using hero target
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="index"></param>
    /// <returns></returns>
	private IEnumerator chargingSpellRoutine(ChargingSpell spell, int index)
	{
		int stage = 0;
		int maxStage = spell.spellStages.Length - 1;
		float chargingTime = 0;
		isCasting = true;
		if (movingCharacter)
			movingCharacter.enableMovement(false);

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;    // fix because camera see point at z = -5

        if (anim)
        {
            anim.SetTrigger("PrepareAttack");
            anim.SetInteger("AttackType", Random.Range(0, 2) + 1);  // Randomizing the attack type
        }

        while (InputManager.instance.IsSpellPressed(spell.spellType))
		{
			if (stage < maxStage)
			{
				setCastBarRatio(chargingTime / spell.spellStages[stage].chargingTime);
				if (chargingTime >= spell.spellStages[stage].chargingTime)
				{
					stage++;
					chargingTime = 0;
					setCastBarRatio(1f);
				}
				else
					chargingTime += Time.deltaTime;
			}
			yield return null;
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;    // fix because camera see point at z = -5
            updateFacingDirection(targetPosition);
        }
		
		isCasting = false;
		if (movingCharacter)
			movingCharacter.enableMovement(true);

        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;    // fix because camera see point at z = -5

        // If it was not charged enough, return without paying mana
        if (spell.castChargingSpell(this, targetPosition, stage))
		{
			StartCoroutine(startCooldown(index));  // Start the cooldown only if the spell has been launched
			payMana(spell.manaCost);
            if (anim)
                anim.SetTrigger("Attack");
            updateFacingDirection(targetPosition);
        }
		else
		{
			if (anim)
				anim.SetTrigger("CancelAttack");
		}

		if (!castingBar)
			yield break;

		yield return new WaitForSeconds(0.25f);
		castingBar.gameObject.SetActive(false);
	}

    private IEnumerator channelSpellRoutine(ChannelSpell spell, int index)
    {
        float chargingTime = 0;
        isCasting = true;

        if (anim)
        {
            anim.SetTrigger("PrepareAttack");
            anim.SetInteger("AttackType", Random.Range(0, 2) + 1);  // Randomizing the attack type
        }

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;    // fix because camera see point at z = -5

        spell.castSpell(this, targetPosition);
        while (InputManager.instance.IsSpellPressed(spell.spellType))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;    // fix because camera see point at z = -5

            updateFacingDirection(targetPosition);

            if (!spell.update(targetPosition))
                break;

            chargingTime += Time.deltaTime;

            yield return null;
        }

        isCasting = false;
        if (movingCharacter)
            movingCharacter.enableMovement(true);

        spell.stop();
        if (anim)
            anim.SetTrigger("CancelAttack");

        StartCoroutine(startCooldown(index));  // Start the cooldown only if the spell has been launched
    }

    private void updateFacingDirection(Vector3 targetPosition)
    {
        if (!anim)
            return;

        Vector2 castDirection = targetPosition - transform.position;
        anim.SetFloat("AttackDirectionX", castDirection.x);
        anim.SetFloat("AttackDirectionY", castDirection.y);
        anim.SetFloat("DirectionX", castDirection.x);
        anim.SetFloat("DirectionY", castDirection.y);
        if (movingCharacter)
            movingCharacter.direction = castDirection;
    }

    /// <summary>
    /// return true if the pays has been processed. False if not enough mana
    /// </summary>
    /// <param name="manaCost"></param>
    /// <param name="payManaInterval"></param>
    /// <returns></returns>
    internal bool payChannelMana(float manaCost, float payManaInterval)
    {
        if (payChannelManaOnCooldown)
            return true;

        if (!payMana(manaCost))
            return false;
        StartCoroutine(startChannelManaCooldown(payManaInterval));
        return true;
    }

    IEnumerator startChannelManaCooldown(float payManaInterval)
    {
        payChannelManaOnCooldown = true;
        yield return new WaitForSeconds(payManaInterval);
        payChannelManaOnCooldown = false;
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

    public bool hasEnoughMana(float manaCost)
    {
        return (manaCost <= mana);
    }

	private bool payMana(float manaCost)
	{
        if (!useMana)
            return true;
        if (manaCost > mana)
            return false;
		if (manaCost <= 0)
			return true;

		mana -= manaCost;
		mana = Mathf.Clamp(mana, 0, maxMana);
        return true;
	}

	internal void giveMana(float manaCost)
	{
		if (manaCost <= 0)
			return;

		mana += manaCost;
		mana = Mathf.Clamp(mana, 0, maxMana);
	}

	public void castOffensiveSpells(Damageable target)
	{
		targetOpponent = target.transform;
		for (int i = 0; i < spellList.Length; i++)
		{
            if (spellList[i].damage >= 0 && spellList[i].shouldCastSpell(this, target))
			    castSpell(i, target.transform.position);
		}
	}

    public bool hasOffensiveSpellsAvailable()
    {
        for (int i = 0; i < spellList.Length; i++)
        {
            if (spellList[i].damage >= 0 && !isOnCoolDown[i])
                return true;
        }
        return false;
    }

    internal void castHealingSpells(Damageable target)
    {
        targetAlly = target.transform;
        for (int i = 0; i < spellList.Length; i++)
        {
            if (spellList[i].damage < 0)
                castSpell(i, targetAlly.position);
        }
    }

    public bool hasHealingSpellsAvailable()
    {
        for (int i = 0; i < spellList.Length; i++)
        {
            if (spellList[i].damage < 0 && !isOnCoolDown[i])
                return true;
        }
        return false;
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

        for (int i = 0; i < spellIcons.Length; i++)
            tooltips[i] = spellIcons[i].GetComponent<HoveringToolTip>();

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
                tooltips[i].initialize(this, spellList[i]);
			}
		}
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

    public void setManaRegen(int manaRegen)
    {
        itemManaRegen = manaRegen;
    }

    public void setCriticalChance(int critChance)
    {
        itemCritChance = critChance;
    }

    public int getCritChance()
    {
        return baseCritChance + itemCritChance;
    }

	internal void addBuff(PowerUpBuff buff)
	{
		StartCoroutine(addBuffRoutine(buff));
	}

    public bool hasLineOfSight(Vector3 target)
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target, GameManager.instance.layerManager.highBlockingLayer);
        return !hit;
    }

    IEnumerator addBuffRoutine(PowerUpBuff buff)
	{
		FloatingText floatingText = (Instantiate(UIManager.instance.floatingText) as GameObject).GetComponent<FloatingText>();
		String text = buff.element.ToString() + " +" + ((buff.multiplier - 1) * 100) + "%";
		floatingText.initialize(gameObject, text);
		floatingText.speed = -1;
		floatingText.duration = 5;
		floatingText.setColor(UIManager.instance.elementColors[(int)buff.element]);
		activeBuffs.Add(buff);
		GameObject activateAura = Instantiate(SpellManager.instance.auraDisablePrefabs[(int)buff.element], transform.position, Quaternion.identity) as GameObject;
        activateAura.GetComponent<Aura>().initialize();
		activateAura.transform.SetParent(transform);
		yield return new WaitForSeconds(0.5f);
		GameObject aura = Instantiate(SpellManager.instance.auraPrefabs[(int)buff.element], transform.position, Quaternion.identity) as GameObject;
        aura.GetComponent<Aura>().initialize();
		aura.transform.SetParent(transform);
		yield return new WaitForSeconds(Mathf.Max(0, buff.duration -0.5f));
		Destroy(aura);
		GameObject deactivateAura = Instantiate(SpellManager.instance.auraActivatePrefabs[(int)buff.element], transform.position, Quaternion.identity) as GameObject;
        deactivateAura.GetComponent<Aura>().initialize();
		deactivateAura.transform.SetParent(transform);
		activeBuffs.Remove(buff);
	}
}
