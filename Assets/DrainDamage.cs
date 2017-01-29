using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainDamage : DamageListener {

    public float drainRatio = 0.1f;

    public override void onDamage(SpellCaster emitter, Damageable dmg, float damage)
    {
        emitter.GetComponent<Damageable>().heal((int)Mathf.Ceil(drainRatio * damage));
    }
}
