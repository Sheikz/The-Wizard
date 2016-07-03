using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffGiver : MonoBehaviour 
{
    public Buff buff;

    private BuffsReceiver buffReceiver;
    private SpellController spell;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        buffReceiver = GetComponentInParent<BuffsReceiver>();
        applyBuff();
    }

    void applyBuff()
    {
        if (!buffReceiver)
            return;

        buff.timeLeft = spell.duration;
        buff.icon = spell.icon;
        buffReceiver.addBuff(buff);
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        if (buffReceiver)
            buffReceiver.removeBuff(buff);
    }
}
