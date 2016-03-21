using UnityEngine;
using System.Collections;
using System;

public class Damageable : MonoBehaviour
{
    public int maxHP;
    public float invincibilityTime = 0.1f;
    public bool isHealable;
    public GameObject deathAnimation;
    public GameObject healAnimation;
    public Material flashingMaterial;

    private GameObject floatingText;
    private int HP;
    public bool onDamageCooldown = false;
    public bool isInvincible = false;
    private GameObject healingAnimation;
    private SpriteRenderer spriteRenderer;
    private MovingCharacter movingChar;
    private bool isDead = false;
    private int healEffects = 0;
    private Material originalMaterial;
    private bool isSlowed = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        HP = maxHP;
        movingChar = GetComponent<MovingCharacter>();
        floatingText = UIManager.instance.floatingText;
        originalMaterial = spriteRenderer.material;
        isDead = false;
    }
    
    public int getHP()
    {
        return HP;
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

    public void inflictDamage(SpellCaster emitter, int damage)
    {
        GameObject dmgText = Instantiate(floatingText) as GameObject;
        dmgText.GetComponent<FloatingText>().initialize(gameObject, damage);

        if (movingChar)
            movingChar.receivesDamage();

        HP -= damage;
        if (HP <= 0)
        {
            ExperienceHolder xpHolder = GetComponent<ExperienceHolder>();
            if (xpHolder && emitter) // If I have experience, give it
                xpHolder.die(emitter.gameObject);

            die();
            return;
        }

        StartCoroutine(damageCooldown(invincibilityTime)); // Make invincible
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
        Instantiate(deathAnimation, transform.position, Quaternion.identity);
        Destroy(gameObject);
        isDead = true;
    }

    /// <summary>
    /// Heal for a certain amount, and create a green floating text
    /// </summary>
    /// <param name="life"></param>
    public void heal(int life)
    {
        if (life <= 0)
            return;

        HP += life;
        if (HP >= maxHP)
            HP = maxHP;

        GameObject healText = Instantiate(floatingText) as GameObject;
        healText.GetComponent<FloatingText>().initialize(gameObject,  life);
        healText.GetComponent<FloatingText>().setColor(Color.green);
    }

    public float getHPRatio()
    {
        return (float)HP / (float)maxHP;
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
        maxHP += additionalHP;
        HP += additionalHP;
        if (HP >= maxHP)
            HP = maxHP;
    }

    public void multiplyMaxHP(float value)
    {
        maxHP = Mathf.RoundToInt(maxHP * value);
        HP = Mathf.RoundToInt(HP * value);

        if (HP >= maxHP)
            HP = maxHP;
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
