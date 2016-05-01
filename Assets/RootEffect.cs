using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RootEffect : StatusEffect
{
    public override void inflictStatus(StatusEffectReceiver dmg)
    {
        StaticSpell staticSpell = GetComponent<StaticSpell>();
        float duration = 0;
        if (staticSpell)
            duration = staticSpell.durationLeft;

        dmg.applyRoot(duration);
    }
}
