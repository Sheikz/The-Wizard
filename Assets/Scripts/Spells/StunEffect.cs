using UnityEngine;

public class StunEffect : StatusEffect
{
    public float stunDuration;

    public override void inflictStatus(StatusEffectReceiver dmg)
    {
        dmg.stunFor(stunDuration);
    }
}
