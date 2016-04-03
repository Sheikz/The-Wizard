using UnityEngine;

public class StunEffect : StatusEffect
{
    public float stunDuration;

    public override void inflictStatus(Damageable dmg)
    {
        MovingCharacter movingChar = dmg.GetComponent<MovingCharacter>();
        if (movingChar)
        {
            movingChar.stunFor(stunDuration);
        }
    }
}
