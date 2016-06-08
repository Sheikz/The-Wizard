using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Buff
{
    public string name;
    public string description;
    public Sprite icon;
    public float timeLeft;
}

public class BuffsReceiver : MonoBehaviour 
{
    private List<Buff> activeBuffs;

    void Awake()
    {
        activeBuffs = new List<Buff>();
    }

    void FixedUpdate()
    {
        for (int i = activeBuffs.Count -1; i >= 0; i--)
        {
            if (activeBuffs[i].timeLeft <= 0)
            {
                activeBuffs.RemoveAt(i);
                continue;
            }
            activeBuffs[i].timeLeft -= Time.fixedDeltaTime;
        }
    }

    public void addBuff(Buff buff)
    {
        if (!containsBuff(buff))
        {
            activeBuffs.Add(buff);
            UIManager.instance.buffBar.addBuff(buff);
        }
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
}
