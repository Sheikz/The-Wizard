using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class Explosion : MonoBehaviour
{
	[Tooltip("Time before the explosion start doing damage")]
	public float delayBeforeExplosion;
	[Tooltip("Time the explosion does damage")]
	public float explosionTime;
    public float lightFadeDuration = 0.5f;

    [HideInInspector]
    public int damage;
    [HideInInspector]
    public SpellCaster emitter;

    private CircleCollider2D circleCollider;
    private SpellIntensity intensity;
    private float manaCost;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
    }

	void Start()
	{
		StartCoroutine(enableAfterSeconds(delayBeforeExplosion));
        setupLights();
	}

    void setupLights()
    {
        foreach (Light light in GetComponentsInChildren<Light>())
        {
            light.transform.localPosition = new Vector3(0, 0, -1);
            light.range = 4;
            light.enabled = true;
            if (UIManager.instance.getFPS() < 60f)
                light.renderMode = LightRenderMode.Auto;
            else
                light.renderMode = LightRenderMode.ForcePixel;
            switch (intensity)
            {
                case SpellIntensity.Tiny: light.intensity = 2; break;
                case SpellIntensity.Small: light.intensity = 2; break;
                case SpellIntensity.Normal: light.intensity = 2; break;
                case SpellIntensity.Mega: light.intensity = 2; break;
            }
        }
    }

    void FixedUpdate()
	{
        if (transform.GetComponentsInChildren<ParticleEmitter>().Length > 0)
            return; // Legacy Particle System can be defined as one-shot and dont need to be destroyed manually

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        if (partSystems.Length == 0)
        {
            StartCoroutine(fadeLights(lightFadeDuration));
        }

        foreach(ParticleSystem ps in partSystems)
        {
            if (!ps.IsAlive())
                Destroy(ps.gameObject);
        }
	}

    IEnumerator fadeLights(float duration)
    {
        float startingTime = Time.time;
        Light[] lights = GetComponentsInChildren<Light>();
        //float startingIntensity = 0;
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

	public void initialize(SpellController spell)
	{
		emitter = spell.emitter;
        damage = spell.damage;
        intensity = spell.lightIntensity;
        manaCost = spell.manaCost;
        gameObject.layer = spell.gameObject.layer;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		GameObject other = collider.gameObject;
		if (other == emitter)
			return;

		Damageable dmg = other.gameObject.GetComponent<Damageable>();
		if (dmg)
		{
			dmg.doDamage(emitter, damage);
            giveMana();
            foreach (StatusEffect effect in GetComponents<StatusEffect>())
            {
                effect.inflictStatus(dmg);
            }
		}
	}

	private IEnumerator enableAfterSeconds(float delay)
	{
		circleCollider.enabled = false;
		yield return new WaitForSeconds(delay);
		circleCollider.enabled = true;
		yield return StartCoroutine(disableAfterSeconds(explosionTime));
	}

	private IEnumerator disableAfterSeconds(float delay)
	{
		if (delay == 0)
			yield return new WaitForSeconds(0.05f);
		else
			yield return new WaitForSeconds(delay);
		circleCollider.enabled = false;
	}

    private void giveMana()
    {
        if (manaCost >= 0)
            return;

        if (emitter)
            emitter.giveMana(-1 * manaCost);
    }
}
