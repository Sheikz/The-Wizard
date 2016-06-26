using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BuffType { Buff, Slow, Freeze, Stun, Root };

[System.Serializable]
public class Buff
{
    public string name;
    public BuffType buffType = BuffType.Buff;
    public string description;
    public Sprite icon;
    public Color colorMask = Color.white;
    public bool timedBuff = true;
    public float timeLeft;
    public float damageMultiplier = 1f;
    public float incomingDamageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public bool stun = false;

    public Buff()
    {
        name = "undefined";
        buffType = BuffType.Buff;
        colorMask = Color.white;
        description = "";
        icon = null;
        timedBuff = true;
        timeLeft = 0;
        damageMultiplier = 1f;
        incomingDamageMultiplier = 1f;
        speedMultiplier = 1f;
        stun = false;
    }

    public Buff(BuffType buffType, float duration, float speedMultiplier = 1f)
    {
        this.buffType = buffType;
        this.timeLeft = duration;
        switch (buffType)
        {
            case BuffType.Freeze:
            case BuffType.Slow:
                this.speedMultiplier = speedMultiplier;
                this.name = "Slow";
                this.icon = SpellManager.instance.slowDebuffIcon;
                this.description = "Slowed by " + (1 - this.speedMultiplier)*100 + "%";
                if (buffType == BuffType.Freeze) this.colorMask = SpellManager.instance.freezeColorMask;
                break;
            case BuffType.Stun:
                this.stun = true;
                this.name = "Stun";
                this.icon = SpellManager.instance.stunIcon;
                this.description = "Stunned";
                break;
            case BuffType.Root:
                this.name = "Root";
                this.description = "Rooted";
                this.speedMultiplier = 0f;
                break;
        }
    }

    public static bool operator ==(Buff a, Buff b)
    {
        return (a.name == b.name &&
                a.buffType == b.buffType &&
                a.timedBuff == b.timedBuff &&
                a.incomingDamageMultiplier == b.incomingDamageMultiplier &&
                a.speedMultiplier == b.speedMultiplier &&
                a.stun == b.stun &&
                a.damageMultiplier == b.damageMultiplier);
    }

    public static bool operator !=(Buff a, Buff b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}