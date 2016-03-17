using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A spell that damages enemies and accumulate drained spell on its way
/// </summary>
[RequireComponent(typeof(SpellController))]
public class DrainSpell : MonoBehaviour
{
    [Range(0, 1f)]
    [Tooltip("The ratio of damage absorbed as drain")]
    public float drainRatio = 0.1f;

    private float damageAbsorbed = 0f;

    public void absorbDamage(float damage)
    {
        damageAbsorbed += damage * drainRatio;
    }

    public void healDamageAbsorbed(Damageable dmg)
    {
        dmg.heal(Mathf.CeilToInt(damageAbsorbed));
    }

}
