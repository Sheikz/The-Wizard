using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum SpellIntensity { Tiny, Small, Normal, Mega };

public abstract class SpellController : MonoBehaviour, IComparable<SpellController>
{
    public string spellName;
    public string spellTypeDescription;
    public string damageString;
    [Tooltip("Description of the spell appearing in tooltips")]
    public string description;
    public float manaCost;
    public float manaCostInterval = 1f;
    public float castTime = 0f;
    public float cooldown = 0.5f;
    public Sprite icon;
	public SpellType spellType = SpellType.Primary;
	public MagicElement magicElement;
	public SpellSet spellSet = SpellSet.SpellSet1;
	public int damage = 10;
    public SpellIntensity lightIntensity = SpellIntensity.Tiny;
    public bool hasSpellWarning = false;

    public float duration = 0f;
    public bool collidesWithSpells = false;
    public bool collidesWithWalls = true;
    [Tooltip("Collides with monsters and hero")]
    public bool collidesWithBothParties = false;
    public SpellController[] upgradedSpells;

    protected Rigidbody2D rb;
	protected CircleCollider2D circleCollider;
	[HideInInspector]
	public SpellCaster emitter;   // Reference to the caster of the spell
	protected LayerMask blockingLayer;
	[HideInInspector]
	public LayerMask enemyLayer;     // The enemy layer
	private float fadeLightsDuration = 0.5f;
    [HideInInspector]
    public Vector3 target;
    [HideInInspector]
    public MoveSpellCaster moveSpellCaster;
    [HideInInspector]
    public PlayerStats stats;
    protected SpellAutoPilot autoPilot;
    protected MultipleSpells multiSpells;
    protected ChainSpell chainSpell;
    protected SpellDamager spellDamager;
    protected HealArea healArea;
    protected ExplodingSpell explodingSpell;
    new protected ParticleSystem particleSystem;
    private bool hasGivenMana = false;

    [HideInInspector]
	public List<Collider2D> ignoredColliders;   // List of colliders that should be ignored

    protected void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
		circleCollider = GetComponent<CircleCollider2D>();
		if (circleCollider)
			circleCollider.isTrigger = true;
		ignoredColliders = new List<Collider2D>();
        explodingSpell = GetComponent<ExplodingSpell>();
        autoPilot = GetComponent<SpellAutoPilot>();
        multiSpells = GetComponent<MultipleSpells>();
        chainSpell = GetComponent<ChainSpell>();
        spellDamager = GetComponent<SpellDamager>();
        healArea = GetComponent<HealArea>();
        particleSystem = GetComponent<ParticleSystem>();
    }

	protected virtual void Start()
	{
        blockingLayer = GameManager.instance.layerManager.spellBlockingLayer;
        setupLights();
		applyStats();
		applyLayer();
        if (emitter)
            stats = emitter.GetComponent<PlayerStats>();
        applyItemPerks();
        SoundManager.instance.playSound(spellName, gameObject);
        if (transform.parent == null)
            transform.SetParent(GameManager.instance.map.spellHolder);
	}

    protected virtual void applyItemPerks()
    {
        if (!stats)
            return;

        switch (spellName)
        {
            case "Energy Bolt":
                if (autoPilot) autoPilot.activated = stats.getItemPerk(ItemPerk.EnergyBoltAimBot) ? true : false;
                break;
            case "Ice Shard":
                if (multiSpells) multiSpells.activated = stats.getItemPerk(ItemPerk.IceShardMultiply) ? true : false;
                break;
            case "Spark":
                if (chainSpell) chainSpell.activated = stats.getItemPerk(ItemPerk.SparkBounce) ? true : false;
                break;
            case "Sanctuary":
                if (healArea) healArea.activated = stats.getItemPerk(ItemPerk.SanctuaryHeal) ? true : false;
                if (spellDamager) spellDamager.activated = stats.getItemPerk(ItemPerk.SanctuaryDamage) ? true : false;
                break;
            case "ChronoSphere":
                if (spellDamager) spellDamager.activated = stats.getItemPerk(ItemPerk.ChronosphereDamage) ? true : false;
                break;
        }
    }

    public abstract SpellController castSpell(SpellCaster emitter, Vector3 target);

    void setupLights()
	{
		foreach (Light light in GetComponentsInChildren<Light>(true))
		{
			light.transform.localPosition = new Vector3(0, 0, -1);
			light.range = 4;
			light.enabled = true;
			if (UIManager.instance.getFPS() < 60f)
				light.renderMode = LightRenderMode.Auto;
			else
				light.renderMode = LightRenderMode.ForcePixel;
			switch (lightIntensity)
			{
				case SpellIntensity.Tiny: light.intensity = 2; break;
				case SpellIntensity.Small: light.intensity = 3; break;
				case SpellIntensity.Normal: light.intensity = 4; break;
				case SpellIntensity.Mega: light.intensity = 5; break;
			}
		}
	}

    internal float getCooldown(SpellCaster spellCaster)
    {
        if (!spellCaster.playerStats)
            return cooldown;

        if (spellName == ItemPerk.IceSlideNoCD.getSpellName() && spellCaster.playerStats.getItemPerk(ItemPerk.IceSlideNoCD))
            return 0.5f;

        return cooldown;
    }

    internal float getCastTime(SpellCaster spellCaster)
    {
        if (!spellCaster.playerStats)
            return castTime;

        if (spellName == ItemPerk.EnergyBoltInstant.getSpellName() && spellCaster.playerStats.getItemPerk(ItemPerk.EnergyBoltInstant))
            return 0;

        return castTime;
    }

    protected IEnumerator destroyAfterSeconds(float seconds)
	{
        if (seconds == 0)
            yield break;
		yield return new WaitForSeconds(seconds);
		Destroy(gameObject);
	}

	protected IEnumerator enableAfterSeconds(Collider2D collider, float delay)
	{
		collider.enabled = false;
		yield return new WaitForSeconds(delay);
		collider.enabled = true;
	}

	void applyStats()
	{
        damage = getDamage(emitter);
	}

    public int getDamage(SpellCaster emitter)
    {
        return (int)(damage * getMultiplier(emitter));
    }

    public float getMultiplier(SpellCaster emitter)
    {
        float result = 1.0f;
        if (emitter == null)
        {
            Debug.LogError("Emitter not defined for " + name);
            return result;
        }
        CharacterStats charStats = emitter.GetComponent<CharacterStats>();
        if (charStats)
            result = Mathf.RoundToInt(result * charStats.getDamageMultiplier(magicElement));
        Inventory inventory = emitter.GetComponent<Inventory>();
        if (inventory)
            result = Mathf.RoundToInt(result * inventory.getDamageMultiplier(magicElement));

        result = Mathf.RoundToInt(result * emitter.getActiveBuff(magicElement));
        return result;
    }

    protected IEnumerator enableForSecondsAfterDelay(Collider2D collider, float delay, float duration)
	{
		collider.enabled = false;
		yield return new WaitForSeconds(delay);
		collider.enabled = true;
		yield return new WaitForSeconds(duration);
		collider.enabled = false;
	}

	public void rotateAroundY(Vector3 direction, Quaternion downDirection)
	{
		transform.rotation = downDirection;
		float angle = Vector3.Angle(Vector3.down, direction);
		if (direction.x <= 0)
			angle *= -1;
		transform.Rotate(new Vector3(0, angle, 0));
	}

	public void rotateAroundX(Vector3 direction, Quaternion downDirection)
	{
		transform.rotation = downDirection;
		float angle = Vector3.Angle(Vector3.down, direction);
		if (direction.x <= 0)
			angle *= -1;
		transform.Rotate(new Vector3(angle, 0, 0));
	}

	protected void applyLayer()
	{
        if (emitter == null)
        {
            Debug.Log("Emitter not defined for " + name);
            return;
        }
        if (emitter.isMonster && damage >= 0)   // Monster offensive spell
        {
            if (collidesWithSpells)
                gameObject.layer = LayerManager.monsterSpellCollidingWithSpellsInt;
            else
                gameObject.layer = LayerManager.monsterSpellsInt;
            enemyLayer = GameManager.instance.layerManager.heroLayer;
        }
        else if (emitter.isMonster && damage < 0)   // Monster healing spell
        {
            if (collidesWithSpells)
                gameObject.layer = LayerManager.heroSpellCollidingWithSpellsInt;
            else
                gameObject.layer = LayerManager.spellsLayerInt;
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
        }
        else if (!emitter.isMonster && damage >= 0) // Hero offensive spell
        {
            if (collidesWithSpells)
                gameObject.layer = LayerManager.heroSpellCollidingWithSpellsInt;
            else
                gameObject.layer = LayerManager.spellsLayerInt;
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
        }
        else // Hero defensive spell
        {
            if (collidesWithSpells)
                gameObject.layer = LayerManager.monsterSpellCollidingWithSpellsInt;
            else
                gameObject.layer = LayerManager.monsterSpellsInt;
            enemyLayer = GameManager.instance.layerManager.heroLayer;
        }
    }

    public virtual bool canCastSpell(SpellCaster spellCaster, Vector3 initialPos, Vector3 target)
    {
        return true;
    }

    /// <summary>
    /// Check if the spell is upgraded by a perk and return the correct one. Return itself if not
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    internal SpellController getUpgradedSpell(SpellCaster caster)
    {
        if (!caster.playerStats)
            return this;

        if (spellName == ItemPerk.FireBallUpgrade.getSpellName() && caster.playerStats.getItemPerk(ItemPerk.FireBallUpgrade))
        {
            if (upgradedSpells.Length > 0 && upgradedSpells[0])
                return upgradedSpells[0];
        }
        return this;
    }

    protected void checkIfAlive()
	{
		if (transform.GetComponentsInChildren<ParticleEmitter>().Length > 0)
			return; // Legacy Particle System can be defined as one-shot and dont need to be destroyed manually

        if (particleSystem)
        {
            if (!particleSystem.IsAlive(true))
                StartCoroutine(fadeLights(fadeLightsDuration));
            return;
        }

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
		if (partSystems.Length == 0)
		{
			StartCoroutine(fadeLights(fadeLightsDuration));
		}

		foreach (ParticleSystem ps in partSystems)
		{
            if (!ps.IsAlive())
                Destroy(ps.gameObject);
		}
	}

	IEnumerator fadeLights(float duration)
	{
		float startingTime = Time.time;
		Light[] lights = GetComponentsInChildren<Light>();
		if (lights.Length > 0)
		{
			float startingIntensity = lights[0].intensity;

			while ((Time.time - startingTime) * Time.timeScale < duration)
			{
				foreach (Light light in lights)
				{
					if (light == null)
						continue;
					light.intensity = Mathf.Lerp(startingIntensity, 0, (Time.time - startingTime) * Time.timeScale / duration);
				}
				yield return null;
			}
		}
		Destroy(gameObject);
	}

	public int CompareTo(SpellController other)
	{
        return (int)spellSet - (int)other.spellSet;
	}

    void OnDestroy()
    {
        if (SoundManager.instance)
            SoundManager.instance.stopSoundFromMe(gameObject);
    }

    public void giveMana()
    {
        if (hasGivenMana)
            return;

        if (manaCost >= 0)
            return;

        emitter.giveMana(-1 * manaCost);
        hasGivenMana = true;
    }

    public virtual bool shouldCastSpell(SpellCaster spellCaster, Damageable target)
    {
        return true;
    }

    protected bool hasEnemiesAround(SpellCaster spellCaster, float radius)
    {
        LayerMask enemyLayer = spellCaster.isMonster ? GameManager.instance.layerManager.heroLayer : GameManager.instance.layerManager.monsterLayer;
        return Physics2D.OverlapCircle(spellCaster.transform.position, radius, enemyLayer);
    }
}
