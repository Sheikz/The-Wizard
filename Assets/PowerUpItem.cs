using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public struct PowerUpBuff
{
    public MagicElement element;
    public string name;
    public float duration;
    public float multiplier;
}

public class PowerUpItem : Item
{
    public PowerUpBuff powerUpBuff;

    private Buff buff;

    new void Start()
    {
        base.Start();
        buff = new Buff();
        buff.name = powerUpBuff.name;
        buff.description = "Increase damage done by " + powerUpBuff.element.ToString() + " spells by " 
            + "<color=orange>"+Mathf.RoundToInt(((powerUpBuff.multiplier - 1) * 100)) + "%</color>";
        buff.timeLeft = powerUpBuff.duration;
        buff.icon = SpellManager.instance.elementIcons[(int)powerUpBuff.element];
    }

    public override void isPickedUpBy(Inventory looter)
    {
        SpellCaster spellCaster = looter.GetComponent<SpellCaster>();
        if (!spellCaster)
            return;

        BuffsReceiver bReceiver = looter.GetComponent<BuffsReceiver>();
        if (bReceiver)
            bReceiver.addBuff(buff);
        spellCaster.addBuff(powerUpBuff);
        Destroy(gameObject);
    }
}
