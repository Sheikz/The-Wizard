using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HealArea : MonoBehaviour
{
    public bool activated = true;
    public float healRatio = 0.025f;
    public float healInterval = 0.5f;

    private List<Damageable> onCooldownList;

    void Awake()
    {
        onCooldownList = new List<Damageable>();
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!activated)
            return;

        Damageable dmg = collision.GetComponent<Damageable>();

        if (dmg && !onCooldownList.Contains(dmg) && dmg.isHealable && !dmg.isDead)
        {
            dmg.healRatio(healRatio);
            StartCoroutine(dmgOnCooldown(dmg));
        }
    }

    private IEnumerator dmgOnCooldown(Damageable dmg)
    {
        onCooldownList.Add(dmg);
        yield return new WaitForSeconds(healInterval);
        onCooldownList.Remove(dmg);
    }
}
