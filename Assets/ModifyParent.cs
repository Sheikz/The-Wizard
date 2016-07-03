using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModifyParent : MonoBehaviour 
{
    private Light[] lights;
    private Damageable dmg;

    void Start()
    {
        if (transform.parent)
            lights = transform.parent.GetComponentsInChildren<Light>();
        dmg = GetComponentInParent<Damageable>();

        if (dmg)
            dmg.isInvincible++;
        foreach (Light l in lights)
            l.enabled = false;
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        if (dmg)
            dmg.isInvincible--;
        foreach (Light l in lights)
            l.enabled = true;
    }
}
