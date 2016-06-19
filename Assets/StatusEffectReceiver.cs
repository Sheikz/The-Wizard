using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StatusEffectReceiver : MonoBehaviour 
{
    public bool[] imunizedTo;

    // Contain the slow percent, along with the number of slow applied as a semaphore
    private Dictionary<float, int> freezeEffects;
    private int colorMaskSemaphore = 0;
    private int stunSemaphore = 0;
    private Color lastColorApplied;
    private Animator anim;
    private Rigidbody2D rb;
    private BuffsReceiver buffReceiver;
    private bool imunizedToAll = false;

    void Awake()
    {
        imunizedTo = new bool[Enum.GetValues(typeof(StatusEffectType)).Length];
        for (int i = 0; i < imunizedTo.Length; i++)
            imunizedTo[i] = false;

        freezeEffects = new Dictionary<float, int>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        buffReceiver = GetComponent<BuffsReceiver>();
    }

    public void applySlow(float moveSpeedPercent, float duration)
    {
        if (moveSpeedPercent == 0)  // It's a stun
        {
            if (isImunizedTo(StatusEffectType.Freeze))
                return;
            stunFor(duration);
            addFreezeDebuff(duration);
            return;
        }
        if (isImunizedTo(StatusEffectType.Slow))
            return;
        StartCoroutine(slowForSeconds(moveSpeedPercent, duration));
    }

    private void addFreezeDebuff(float duration)
    {
        if (!buffReceiver)
            return;

        Buff freezeDebuff = new Buff();
        freezeDebuff.name = "Freeze";
        freezeDebuff.icon = SpellManager.instance.freezeDebuffIcon;
        freezeDebuff.timeLeft = duration;
        buffReceiver.addBuff(freezeDebuff);
    }

    private void addSlowDebuff(float duration)
    {
        if (!buffReceiver)
            return;

        Buff freezeDebuff = new Buff();
        freezeDebuff.name = "Slow";
        freezeDebuff.icon = SpellManager.instance.slowDebuffIcon;
        freezeDebuff.timeLeft = duration;
        buffReceiver.addBuff(freezeDebuff);
    }

    internal void setImunized(bool v)
    {
        imunizedToAll = v;
    }

    private bool isImunizedTo(StatusEffectType type)
    {
        if (imunizedToAll)
            return true;
        return imunizedTo[(int)type];
    }

    public void applySlow(float moveSpeedPercent, Color colorMask, float duration)
    {
        if (moveSpeedPercent == 0)  // It's a stun
        {
            stunFor(duration, colorMask);
            addFreezeDebuff(duration);
            return;
        }
        if (isImunizedTo(StatusEffectType.Slow))
            return;

        StartCoroutine(slowForSeconds(moveSpeedPercent, duration));
        applyColorMask(colorMask, duration);
    }

    /// <summary>
    /// Root is just a 100% slow
    /// </summary>
    /// <param name="duration"></param>
    public void applyRoot(float duration)
    {
        if (isImunizedTo(StatusEffectType.Root))
            return;

        StartCoroutine(slowForSeconds(0, duration));
    }

    /// <summary>
    /// Slow the movement and animation speed of target. If slow = 0, it's a root and it should not slow down animations
    /// </summary>
    /// <param name="moveSpeedPercent"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator slowForSeconds(float moveSpeedPercent, float duration)
    {
        addSlowDebuff(duration);
        if (freezeEffects.ContainsKey(moveSpeedPercent))
            freezeEffects[moveSpeedPercent]++;
        else
        {
            freezeEffects.Add(moveSpeedPercent, 1);
            if (moveSpeedPercent != 0)
                anim.speed *= moveSpeedPercent;
        }

        yield return new WaitForSeconds(duration);

        freezeEffects[moveSpeedPercent]--;
        if (freezeEffects[moveSpeedPercent] == 0)
        {
            freezeEffects.Remove(moveSpeedPercent);
            if (moveSpeedPercent != 0)
                anim.speed /= moveSpeedPercent;
        }
    }

    /// <summary>
    /// Return the minimum movespeed in the ones currently applied
    /// </summary>
    /// <returns></returns>
    public float getMoveSpeedPercent()
    {
        float result = 1f;
        foreach (float value in freezeEffects.Keys)
        {
            if (value <= result)
                result = value;
        }
        return result;
    }

    internal void stunFor(float stunDuration)
    {
        if (isImunizedTo(StatusEffectType.Stun))
            return;
        StartCoroutine(stunRoutine(stunDuration));
    }

    internal void stunFor(float stunDuration, Color colorMask)
    {
        if (isImunizedTo(StatusEffectType.Stun))
            return;
        StartCoroutine(stunRoutine(stunDuration));
        applyColorMask(colorMask, stunDuration);
    }

    IEnumerator stunRoutine(float stunDuration)
    {
        stunSemaphore++;
        if (anim)
            anim.speed = 0;
        if (rb)
            rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);
        stunSemaphore--;
        if (stunSemaphore == 0)
            anim.speed = 1;
    }

    public bool isStunned
    {
        get
        {
            if (stunSemaphore > 0)
                return true;
            else
                return false;
        }
    }

    public void applyColorMask(Color color, float duration)
    {
        StartCoroutine(applyColorMaskRoutine(color, duration));
    }

    private IEnumerator applyColorMaskRoutine(Color color, float duration)
    {
        SpriteRenderer[] spriteRenderers;
        if (lastColorApplied != color)
        {
            lastColorApplied = color;
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sp in spriteRenderers)
            {
                color.a = sp.color.a;
                sp.color = color;
            }
        }

        colorMaskSemaphore++;
        yield return new WaitForSeconds(duration);
        colorMaskSemaphore--;

        if (colorMaskSemaphore == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sp in spriteRenderers)
            {
                Color white = Color.white;
                white.a = sp.color.a;
                sp.color = white;
            }
        }
    }

}
