using UnityEngine;
using System.Collections;

public class FreezeEffect : StatusEffect 
{
    public float slowPercent = 0.5f;
    public float duration = 3;
    public Color colorMask;

    public override void inflictStatus(StatusEffectReceiver target)
    {
        target.applySlow(slowPercent, colorMask, duration);
    }
}