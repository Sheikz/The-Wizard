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
    public float cooldown = 0.2f;
    public Sprite icon;
	public SpellType spellType = SpellType.Primary;
	public MagicElement magicElement;
	public SpellSet spellSet = SpellSet.SpellSet1;
	public int damage = 10;
	public SpellIntensity lightIntensity = SpellIntensity.Tiny;
    public float duration = 0f;
    public bool chargingSpell = false; // Can this spell be charged?
    
	[Tooltip("The list of spells required to unlock this one")]
	public GameObject[] prerequisites;


    [Tooltip("Should the prerequisites be removed when the spell is learned?")]
	public bool removePrerequisites = false;

	protected Rigidbody2D rb;
	protected CircleCollider2D circleCollider;
	[HideInInspector]
	public SpellCaster emitter;   // Reference to the caster of the spell
	protected LayerMask blockingLayer;
	[HideInInspector]
	public LayerMask enemyLayer;     // The enemy layer
	private float fadeLightsDuration = 0.5f;

	public bool collidesWithWalls = true;
	[HideInInspector]
	public List<Collider2D> ignoredColliders;   // List of colliders that should be ignored

    protected void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		circleCollider = GetComponent<CircleCollider2D>();
		if (circleCollider)
			circleCollider.isTrigger = true;
		blockingLayer = GameManager.instance.layerManager.spellBlockingLayer;
		ignoredColliders = new List<Collider2D>();
    }

	protected void Start()
	{
		setupLights();
		applyStats();
		applyLayer();
	}

	public abstract SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target);

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

	protected IEnumerator destroyAfterSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Destroy(gameObject);
	}

	protected IEnumerator enableAfterSeconds(Collider2D collider, float delay)
	{
		collider.enabled = false;
		yield return new WaitForSeconds(delay);
		collider.enabled = true;
	}

	public void applyStats()
	{
		if (emitter == null)
		{
			Debug.Log("Emitter not defined for " + name);
			return;
		}
		CharacterStats stats = emitter.GetComponent<CharacterStats>();
		if (stats == null)
		{
			Debug.Log("No stats found for " + name);
			return;
		}
        damage = getDamage(stats);
	}

    public int getDamage(CharacterStats stats)
    {
        return Mathf.RoundToInt(damage * stats.getDamageMultiplier(magicElement));
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
		if (emitter.isMonster)
		{
			gameObject.layer = LayerMask.NameToLayer("MonsterSpells");
			enemyLayer = GameManager.instance.layerManager.heroLayer;
		}
		else
		{
			gameObject.layer = LayerMask.NameToLayer("Spells");
			enemyLayer = GameManager.instance.layerManager.monsterLayer;
		}
	}

    public virtual bool canCastSpell(SpellCaster spellCaster, Vector3 initialPos, Vector3 target)
    {
        return true;
    }

    protected void checkIfAlive()
	{
		if (transform.GetComponentsInChildren<ParticleEmitter>().Length > 0)
			return; // Legacy Particle System can be defined as one-shot and dont need to be destroyed manually

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
}
