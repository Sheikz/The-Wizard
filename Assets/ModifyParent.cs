using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModifyParent : MonoBehaviour 
{
    public Color colorMask;

    private SpriteRenderer rdr;
    private Light[] lights;
    private Damageable dmg;

    void Start()
    {
        rdr = GetComponentInParent<SpriteRenderer>();
        if (transform.parent)
            lights = transform.parent.GetComponentsInChildren<Light>();
        dmg = GetComponentInParent<Damageable>();

        if (rdr)
            rdr.color = colorMask;
        if (dmg)
            dmg.isInvincible++;
        foreach (Light l in lights)
            l.enabled = false;
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        if (rdr)
            rdr.color = Color.white;
        if (dmg)
            dmg.isInvincible--;
        foreach (Light l in lights)
            l.enabled = true;
    }
}
