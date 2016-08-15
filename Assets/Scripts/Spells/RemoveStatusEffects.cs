using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RemoveStatusEffects : MonoBehaviour 
{
    private BuffsReceiver buffReceiver;

    void Start()
    {
        buffReceiver = GetComponentInParent<BuffsReceiver>();
        if (!buffReceiver)
            return;

        buffReceiver.removeAllDebuffs();
        buffReceiver.setImunizedDebuffs(true);
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        if (buffReceiver)
            buffReceiver.setImunizedDebuffs(false);
    }
}
