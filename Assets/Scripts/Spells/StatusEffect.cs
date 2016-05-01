using UnityEngine;
using System.Collections;

public enum StatusEffectType { Slow, Root, Stun, Burning, Knockback};

public abstract class StatusEffect : MonoBehaviour 
{
    public abstract void inflictStatus(StatusEffectReceiver receiver);
}
