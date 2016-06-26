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

    void Awake()
    {
        activeBuffs = new List<Buff>();
    }

    void FixedUpdate()
    {
        bool shouldRefresh = false;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (!activeBuffs[i].timedBuff)
                continue;

            if (activeBuffs[i].timeLeft <= 0)
            {
                activeBuffs.RemoveAt(i);
                shouldRefresh = true;
                continue;
            }
            activeBuffs[i].timeLeft -= Time.fixedDeltaTime;
        }
        if (shouldRefresh)
            refreshBuffs();
    }

    internal List<Buff> getBuffList()
    {
        return activeBuffs;
    }

    public void addBuff(Buff buff)
    {
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
        refreshBuffs();
    }

    private void refreshBuffs()
    {
        incomingDamageMultiplier = 1; // 1 means no damage reduction, 0 means 100% damage reduction, 1.5 means +50% incoming damage
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        isStunned = false;
        foreach (Buff buff in activeBuffs)
        {
            if (buff.stun)
                isStunned = true;

            incomingDamageMultiplier *= buff.incomingDamageMultiplier;
            damageMultiplier *= buff.damageMultiplier;
            speedMultiplier *= buff.speedMultiplier;
        }
    }

    internal void stunFor(float fallingDuration)
    {
        Buff newBuff = new Buff(BuffType.Stun, fallingDuration);
        addBuff(newBuff);
    }
}
