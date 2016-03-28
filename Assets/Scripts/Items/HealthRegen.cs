using UnityEngine;
using System.Collections;

public class HealthRegen : Item
{
    public float lifeRatio;
    public float duration;

    override public void isPickedUpBy(Inventory other)
    {
        Damageable dmg = other.GetComponent<Damageable>();
        if (dmg && dmg.isHealable)
        {
            dmg.healRatioOverTime(lifeRatio, duration);
            Destroy(gameObject);
        }
    }
}
