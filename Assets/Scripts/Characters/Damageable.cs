using UnityEngine;
using System.Collections;
using System;

public class Damageable : MonoBehaviour
{
    public int baseHP;
    public float invincibilityTime = 0.1f;
    public bool isHealable;
    public GameObject healAnimation;
    public Material flashingMaterial;
    public bool showHPBar = true;

    private GameObject floatingText;
    public int maxHP;
    public int currentHP;
    public bool onDamageCooldown = false;
    public bool isInvincible = false;
    private GameObject healingAnimation;
    private SpriteRenderer spriteRenderer;
    private MovingCharacter movingChar;
    [HideInInspector]
    public bool isDead = false;
    private int healEffects = 0;
    private Material originalMaterial;
    private bool isSlowed = false;
    private FloatingHPBar floatingHPBar;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxHP = baseHP;
        currentHP = maxHP;
        movingChar = GetComponent<MovingCharacter>();
        floatingText = UIManager.instance.floatingText;
        originalMaterial = spriteRenderer.material;
        isDead = false;
    }
    
    public int getHP()
    {
        return currentHP;
    }

    public void doDamage(SpellCaster emitter, int damage)
    {
        if (isDead)
            return;

        if (isInvincible)
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
        GameObject dmgText = Instantiate(floatingText) as GameObject;
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
            
        isDead = true;
    }

    public int missingLife()
    {
        if (isDead)
            return 0;

        return maxHP - currentHP;
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
        healText.GetComponent<FloatingText>().initialize(gameObject,  life);
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
            if (spriteRenderer.material == originalMaterial)
                spriteRenderer.material = flashingMaterial;
            else
                spriteRenderer.material = originalMaterial;

            // Flickering frequence
            yield return new WaitForSeconds(0.05f);

        }
        onDamageCooldown = false;
        spriteRenderer.material = originalMaterial;
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

    public void applyColorMask(Color color, float duration)
    {
        if (isSlowed)
            return;
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sp in spriteRenderers)
        {
            StartCoroutine(applyColorMask(sp, color, duration));
        }
    }

    private IEnumerator applyColorMask(SpriteRenderer sp, Color color, float duration)
    {
        isSlowed = true;
        Color originalColor = sp.color;
        sp.color = color;
        yield return new WaitForSeconds(duration);
        if (!sp)
            yield break;
        sp.color = originalColor;
        isSlowed = false;
    }

}
