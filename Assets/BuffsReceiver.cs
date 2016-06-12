using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuffsReceiver : MonoBehaviour 
{
    private List<Buff> activeBuffs;
    private float damageReduction = 1;

    void Awake()
    {
        activeBuffs = new List<Buff>();
    }

    void FixedUpdate()
    {
        bool shouldRefresh = false;

        for (int i = activeBuffs.Count -1; i >= 0; i--)
        {
            if (!activeBuffs[i].timedBuff)
                continue;

            if (activeBuffs[i].timeLeft <= 0)
            {
                removeBuff(activeBuffs[i]);
                shouldRefresh = true;
                continue;
            }
            activeBuffs[i].timeLeft -= Time.fixedDeltaTime;
        }
        if (shouldRefresh)
            refreshBuffs();
    }

    public void addBuff(Buff buff)
    {
        if (!containsBuff(buff))
        {
            activeBuffs.Add(buff);
            UIManager.instance.buffBar.addBuff(buff);
        }
        refreshBuffs();
    }

    public void removeBuff(Buff buff)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (activeBuffs[i].name == buff.name)
            {
                activeBuffs.RemoveAt(i);
                UIManager.instance.buffBar.removeBuff(buff);
            }
        }
        refreshBuffs();
    }

    private bool containsBuff(Buff b)
    {
        foreach (Buff buff in activeBuffs)
        {
            if (b.name == buff.name)
                return true;
        }
        return false;
    }

    private void refreshBuffs()
    {
        damageReduction = 1; // 1 means no damage reduction, 0 means 100% damage reduction
        foreach (Buff buff in activeBuffs)
        {
            damageReduction *= (1 - buff.damageReduction);
        }
    }

    internal float getDamageReduction()
    {
        return damageReduction;
    }
}
