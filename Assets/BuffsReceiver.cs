using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuffsReceiver : MonoBehaviour 
{
    public List<Buff> activeBuffs;
    [HideInInspector]
    public float incomingDamageMultiplier = 1;
    [HideInInspector]
    public float damageMultiplier = 1;
    [HideInInspector]
    public float speedMultiplier = 1f;
    [HideInInspector]
    public bool isStunned = false;

    private bool[] imunizedTo;
    private Color colorMask = Color.white;
    private SpriteRenderer spr;

    void Awake()
    {
        activeBuffs = new List<Buff>();
        spr = GetComponent<SpriteRenderer>();
        imunizedTo = new bool[Enum.GetValues(typeof(BuffType)).Length];
        for (int i = 0; i < imunizedTo.Length; i++)
            imunizedTo[i] = false;
    }

    void FixedUpdate()
    {
        bool shouldRefresh = false;
        bool refreshColor = false;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (!activeBuffs[i].timedBuff)
                continue;

            if (activeBuffs[i].timeLeft <= 0)
            {
                if (activeBuffs[i].colorMask != Color.white)
                    refreshColor = true;
                activeBuffs.RemoveAt(i);
                shouldRefresh = true;
                continue;
            }
            activeBuffs[i].timeLeft -= Time.fixedDeltaTime;
        }
        if (shouldRefresh)
            refreshBuffs(refreshColor);
    }

    internal List<Buff> getBuffList()
    {
        return activeBuffs;
    }

    public void addBuff(Buff buff)
    {
        if (buff.resistable && imunizedTo[(int)buff.buffType]) // Don't add the buff if I'm immune
            return;

        for (int i = activeBuffs.Count-1; i >= 0; i--)
        {
            if (activeBuffs[i] == buff)
            {
                if (buff.timedBuff && buff.timeLeft >= activeBuffs[i].timeLeft)
                {
                    activeBuffs[i].timeLeft = buff.timeLeft;    // Refreshing the duration if it's the same buff
                    return;
                }
            }
        }
        applyColor(buff);
        activeBuffs.Add(buff);
        refreshBuffs();
    }

    public void removeBuff(Buff buff)
    {
        for (int i=activeBuffs.Count -1; i >= 0; i--)
        {
            if (activeBuffs[i] == buff)
                activeBuffs.RemoveAt(i);
        }
        refreshBuffs(buff.colorMask != Color.white);    // If this buff was applying a color mask, need to 
    }

    private void refreshBuffs(bool refreshColor = false)
    {
        float longestColorTimeleft = -1f;
        Color colorWithLongestTimeLeft = Color.white;
        incomingDamageMultiplier = 1; // 1 means no damage reduction, 0 means 100% damage reduction, 1.5 means +50% incoming damage
        damageMultiplier = 1f;
        float speedDebuffMultiplier = 1f;
        float speedBuffMultiplier = 1f;
        isStunned = false;
        foreach (Buff buff in activeBuffs)
        {
            if (buff.stun)
                isStunned = true;
            if (refreshColor)   // If the color has to be refreshed, check if there is another buff currently applied that has a color mask.
            {
                if (buff.colorMask != colorWithLongestTimeLeft && buff.timeLeft >= longestColorTimeleft)    // Take the one with the most time left
                {
                    colorWithLongestTimeLeft = buff.colorMask;
                    longestColorTimeleft = buff.timeLeft;
                }
            }
            incomingDamageMultiplier *= buff.incomingDamageMultiplier;
            damageMultiplier *= buff.damageMultiplier;
            if (buff.buffType != BuffType.Buff && buff.speedMultiplier <= speedDebuffMultiplier)        // Taking the biggest slow debuff into account
                speedDebuffMultiplier = buff.speedMultiplier;
            if (buff.buffType == BuffType.Buff && buff.speedMultiplier >= speedBuffMultiplier)          // And the biggest speed boost
                speedBuffMultiplier = buff.speedMultiplier;
        }
        speedMultiplier = speedDebuffMultiplier * speedBuffMultiplier;
        if (refreshColor && spr)
            spr.color = colorWithLongestTimeLeft;
    }

    void applyColor(Buff buff)
    {
        if (buff.colorMask != Color.white)
            spr.color = buff.colorMask;
    }

    internal void stunFor(float duration)
    {
        Buff newBuff = new Buff(BuffType.Stun, duration);
        addBuff(newBuff);
    }

    internal void removeAllDebuffs()
    {
        for (int i=activeBuffs.Count-1; i >= 0; i--)
        {
            if (activeBuffs[i].buffType != BuffType.Buff && activeBuffs[i].removable)   // It's a debuff
            {
                activeBuffs.RemoveAt(i);
            }
        }
        refreshBuffs();
    }

    internal void setImunizedDebuffs(bool v)
    {
        for (int i = 0; i < imunizedTo.Length; i++)
            imunizedTo[i] = v;

        imunizedTo[(int)BuffType.Buff] = false; // Never immune to buffs
    }

}
