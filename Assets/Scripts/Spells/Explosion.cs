using UnityEngine;
using System.Collections;
using System;

public enum DamageValueType { Absolute, Ratio };
[RequireComponent(typeof(CircleCollider2D))]
public class Explosion : MonoBehaviour
{
    [Tooltip("Time before the explosion start doing damage")]
    public float delayBeforeExplosion;
    [Tooltip("Time the explosion does damage")]
    public float explosionTime;

    public float lightFadeDuration = 0.5f;
    public bool hitOnce = false;
    public DamageValueType damageValueType = DamageValueType.Absolute;

    [HideInInspector]
    public int damage;
    [HideInInspector]
    public SpellCaster emitter;
    [HideInInspector]
    public float damageRatio;

    private CircleCollider2D circleCollider;
    private SpellIntensity intensity;
    private float manaCost;
    private string spellName;
    private ParticleSystem partSystem;
    [HideInInspector]
    public SpellController spell;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        partSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        StartCoroutine(enableAfterSeconds(delayBeforeExplosion));
        setupLights();
        transform.SetParent(GameManager.instance.map.spellHolder);
        SoundManager.instance.playSound(spellName + "Hit");
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

        if (partSystem)
        {
            if (!partSystem.IsAlive(true))
                Destroy(gameObject);
            return;
        }

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        if (partSystems.Length == 0)
        {
            StartCoroutine(fadeLights(lightFadeDuration));
        }

        foreach (ParticleSystem ps in partSystems)
        {
            if (!ps.IsAlive(true))
                Destroy(ps.gameObject);
        }
    }

    IEnumerator fadeLights(float duration)
    {
        float startingTime = Time.time;
        Light[] lights = GetComponentsInChildren<Light>(); ;
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

    public virtual void initialize(SpellController spell)
    {
        this.spell = spell;
        spellName = spell.spellName;
        emitter = spell.emitter;
        damage = spell.damage;
        intensity = spell.lightIntensity;
        manaCost = spell.manaCost;
        gameObject.layer = spell.gameObject.layer;
    }

    internal void initialize(ExplodingObject explodingObject)
    {
        
        damageValueType = DamageValueType.Ratio;
        damageRatio = explodingObject.damageRatio;
    }


    /*
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (hitOnce && appliedHit)
            return;

        GameObject other = collider.gameObject;
        if (other == emitter)
            return;

        Damageable dmg = other.gameObject.GetComponent<Damageable>();
        if (dmg)
        {
            if (damage >= 0)
            {
                dmg.doDamage(emitter, damage);
            }
            else
                dmg.heal(-damage);

            appliedHit = true;

            BuffsReceiver receiver = dmg.GetComponent<BuffsReceiver>();
            if (!receiver)
                return;

            foreach (StatusEffect effect in GetComponents<StatusEffect>())
            {
                effect.applyBuff(receiver);
            }
        }
    }*/

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
}
