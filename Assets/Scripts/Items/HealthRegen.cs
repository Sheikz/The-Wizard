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
            SoundManager.instance.playSound("GetHealth");
            dmg.healRatioOverTime(lifeRatio, duration);
            Destroy(gameObject);
        }
    }
}
