using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RemoveStatusEffects : MonoBehaviour 
{
    private StatusEffectReceiverOld statusEffectReceiver;

    void Awake()
    {
        statusEffectReceiver = GetComponentInParent<StatusEffectReceiverOld>();
    }

    void Start()
    {
        if (!statusEffectReceiver)
            return;

        statusEffectReceiver.removeAllDebuffs();
        statusEffectReceiver.setImunized(true);
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        statusEffectReceiver.setImunized(false);
    }
}
