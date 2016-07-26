using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Damageable : MonoBehaviour
{
    public int baseHP;
    public float invincibilityTime = 0.1f;
    public bool isHealable;
    public GameObject healAnimation;
    public Material flashingMaterial;
    public bool showHPBar = true;
    public GameObject[] destructionAnimation;
    public int outOfCombatRegen = 0;
    public float timeToLeaveCombat = 5f;
    public bool isUnit = true;  // Is this a unit of a static object

    private GameObject floatingText;
    public int maxHP;
    public float currentHP;
    public bool onDamageCooldown = false;
    public int isInvincible = 0;    // Semaphore instead of bool to manage multiple sources of invincibility
    private GameObject healingAnimation;
    private SpriteRenderer spriteRenderer;
    private MovingCharacter movingChar;

    [HideInInspector]
    public bool isDead = false;
    private int healEffects = 0;
    private Material originalMaterial;
    private FloatingHPBar floatingHPBar;
    private List<Damager> damagers;

    private BuffsReceiver buffReceiver;
    private SpellAbsorbDamage spellAbsorbDamage;
    public bool inCombat = false;
    public float timeleftInCombat = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movingChar = GetComponent<MovingCharacter>();
        if (spriteRenderer)
            originalMaterial = spriteRenderer.material;
        damagers = new List<Damager>();
        buffReceiver = GetComponent<BuffsReceiver>();
        spellAbsorbDamage = GetComponent<SpellAbsorbDamage>();
    }

    void Start()
    {
        floatingText = UIManager.instance.floatingText;
        maxHP = baseHP;
        currentHP = maxHP;
        isDead = false;
        StartCoroutine(regenHP());
    }

    void FixedUpdate()
    {
        if (timeleftInCombat <= 0)
            return;

        timeleftInCombat -= Time.fixedDeltaTime;

        if (timeleftInCombat <= 0)
        {
            timeleftInCombat = 0f;
            inCombat = false;
        }
    }

    private IEnumerator regenHP()
    {
        while (true)
        {
            if (!inCombat && outOfCombatRegen > 0)
            {
                currentHP += (outOfCombatRegen / 100f * maxHP) /4f;
                if (currentHP >= maxHP)
                    currentHP = maxHP;
                refreshHPBar();
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    public int getHP()
    {
        return Mathf.RoundToInt(currentHP);
    }

    public void doDamage(SpellCaster emitter, int damage)
    {
        if (isDead)
            return;

        if (isInvincible > 0)
            return;

        if (!isDamageable(emitter))  // Am I damaging myself?
            return;

        if (onDamageCooldown)               // Invincible?
            return;

        if (damage <= 0)
        {
            Debug.Log("null damage detected");
            return;
        }

        inflictDamage(emitter, damage);
    }

    internal void doDamage(SpellDamager spellDamager, SpellCaster emitter, int damage)
    {
        if (isDead)
            return;

        if (isInvincible > 0)
            return;

        if (!isDamageable(emitter))  // Am I damaging myself?
            return;

        if (damagers.Contains(spellDamager)) // Already damaged by this object ?
            return;

        StartCoroutine(startDamageCooldown(spellDamager));

        if (spellDamager.damageValueType == DamageValueType.Ratio)
        {
            inflictDamageRatio(spellDamager.relativeDamageRatio);
            return;
        }

        if (damage <= 0)
        {
            Debug.Log("null damage detected");
            return;
        }

        inflictDamage(emitter, damage);
    }

    internal void doDamageRatio(ColliderDamager source, float lifeDamageRatio)
    {
        if (isDead)
            return;

        if (isInvincible > 0)
            return;

        if (damagers.Contains(source)) // Already damaged by this object ?
            return;

        StartCoroutine(startDamageCooldown(source));
        inflictDamageRatio(lifeDamageRatio);
    }

    private IEnumerator startDamageCooldown(Damager damager)
    {
        damagers.Add(damager);
        yield return new WaitForSeconds(invincibilityTime);
        damagers.Remove(damager);
    }

    internal void multiplyBaseHP(float mult, int hpFromItems, bool updateCurrentHP)
    {
        float ratio = 1.0f;
        if (updateCurrentHP)
            ratio = (float)currentHP / maxHP;
        maxHP = Mathf.RoundToInt(baseHP * mult);
        if (updateCurrentHP)
            currentHP = Mathf.RoundToInt(maxHP * ratio);
        maxHP += hpFromItems;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void inflictDamage(SpellCaster emitter, int damage)
    {
        bool criticalHit = false;
        // Is a critical hit?
        int chance = Random.Range(0, 100);
        if (emitter && chance < emitter.getCritChance())
            criticalHit = true;

        GameObject dmgText = null;
        if (floatingText)
            dmgText = Instantiate(floatingText) as GameObject;
        if (criticalHit)
        {
            if (dmgText)
                dmgText.GetComponent<FloatingText>().setCriticalHit();
            damage *= 2;
        }

        // Apply buff damage reduction
        if (buffReceiver)
        {
            damage = Mathf.RoundToInt(damage * buffReceiver.incomingDamageMultiplier);
        }

        if (dmgText)
            dmgText.GetComponent<FloatingText>().initialize(gameObject, damage);

        if (movingChar)
            movingChar.receivesDamage();

        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            ExperienceHolder xpHolder = GetComponent<ExperienceHolder>();
            if (xpHolder && emitter) // If I have experience, give it
                xpHolder.die(emitter.gameObject);

            die();
        }
        refreshHPBar();

        StartCoroutine(damageCooldown(invincibilityTime)); // Make invincible
    }

    /// <summary>
    /// Update the floating HP Bar. Create an new one if there is none
    /// </summary>
    private void refreshHPBar()
    {
        if (!showHPBar)
        {
            if (floatingHPBar)
                floatingHPBar.gameObject.SetActive(false);

            return;
        }

        if (floatingHPBar == null)
        {
            floatingHPBar = Instantiate(UIManager.instance.floatingHPBar);
            floatingHPBar.transform.SetParent(transform);
            floatingHPBar.transform.localPosition = new Vector3(0, 0.8f, 0);
        }

        floatingHPBar.setRatio((float)currentHP / (float)maxHP);
    }

    public void inflictDamageRatio(float ratio)
    {
        if (ratio <= 0)
            return;
        int damage = Mathf.RoundToInt(ratio * maxHP);
        inflictDamage(null, damage);
    }

    public void die()
    {
        if (movingChar)
            movingChar.die();
        else
            Destroy(gameObject);

        ItemHolder holder = GetComponent<ItemHolder>();
        if (holder)
            holder.die();

        foreach (GameObject destAnimation in destructionAnimation)
        {
            if (destAnimation == null)
                continue;
            GameObject destruction = Instantiate(destAnimation, transform.position, Quaternion.identity) as GameObject;
            if (!isUnit)
            {
                destruction.gameObject.layer = LayerManager.instance.monstersAndHeroLayerInt;
            }
        }

        ExplodingObject obj = GetComponent<ExplodingObject>();
        if (obj)
            obj.explode();
            
        isDead = true;
    }

    public int missingLife()
    {
        if (isDead)
            return 0;

        return maxHP - Mathf.RoundToInt(currentHP);
    }

    /// <summary>
    /// Heal for a certain amount, and create a green floating text
    /// </summary>
    /// <param name="life"></param>
    public void heal(int life)
    {
        if (life <= 0)
            return;

        if (life > missingLife())
            life = missingLife();

        if (life == 0)
            return;

        currentHP += life;
        if (currentHP >= maxHP)
            currentHP = maxHP;

        refreshHPBar();
        GameObject healText = Instantiate(floatingText) as GameObject;
        healText.GetComponent<FloatingText>().initialize(gameObject, life);
        healText.GetComponent<FloatingText>().setColor(Color.green);
    }

    public float getHPRatio()
    {
        return (float)currentHP / (float)maxHP;
    }

    /// <summary>
    ///  Can this object damage me?
    /// </summary>
    /// <param name="emitterTag"></param>
    /// <returns></returns>
    private bool isDamageable(SpellCaster emitter)
    {
        if (emitter == null)
            return true;
        if (emitter.gameObject == null)    // If the emitter no longer exists, the damage can surely happen
            return true;
        if (emitter.gameObject == gameObject)  // If the emitter is the same as the receiver, there is no damage
            return false;
        return true;
    }

    /// <summary>
    /// Make the character invincible and flash for the duration
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator damageCooldown(float duration)
    {
        float startingTime = Time.time;
        onDamageCooldown = true;
        while (Time.time - startingTime < duration)
        {
            if (spriteRenderer && spriteRenderer.material == originalMaterial)
                spriteRenderer.material = flashingMaterial;
            else if (spriteRenderer)
                spriteRenderer.material = originalMaterial;

            // Flickering frequence
            yield return new WaitForSeconds(0.05f);
        }
        onDamageCooldown = false;
        if (spriteRenderer)
            spriteRenderer.material = originalMaterial;
    }

    public void healRatio(float ratio)
    {
        heal(Mathf.RoundToInt(maxHP * ratio));
    }

    public void healRatioOverTime(float ratio, float duration)
    {
        healOverTime(Mathf.RoundToInt(maxHP * ratio), duration);
    }

    public void healOverTime(int life, float duration)
    {
        StartCoroutine(healOverTimeRoutine(life, duration));
    }

    private IEnumerator healOverTimeRoutine(int life, float duration)
    {
        if (healAnimation)
        {
            healEffects++;
            if (!healingAnimation)
            {
                healingAnimation = Instantiate(healAnimation, transform.position, Quaternion.identity) as GameObject;
                healingAnimation.transform.SetParent(transform);
            }
        }
        int numberOfTicks = Mathf.RoundToInt(duration * 4); // 4 tics per seconds
        for (int i = 0; i < numberOfTicks; i++)
        {
            heal(Mathf.RoundToInt((float)life / numberOfTicks));
            yield return new WaitForSeconds(duration / numberOfTicks);
        }

        if (healAnimation)
        {
            healEffects--;
            if (healEffects <= 0)
            {
                healEffects = 0;
                Destroy(healingAnimation);
                healingAnimation = null;
            }
        }
    }

    public void increaseMaxHP(int additionalHP)
    {
        Debug.Log("Added " + additionalHP+" hp");
        maxHP += additionalHP;
    }

    public void setInvincible(bool v)
    {
        if (v)
            isInvincible++;
        else
            isInvincible--;
    }

    public void setInCombat(bool value)
    {
        inCombat = value;
        timeleftInCombat = value ? timeToLeaveCombat : 0f;
    }
}
