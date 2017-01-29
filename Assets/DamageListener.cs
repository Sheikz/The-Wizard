using UnityEngine;

public abstract class DamageListener : MonoBehaviour
{
    public abstract void onDamage(SpellCaster emitter, Damageable dmg, float damage);

}