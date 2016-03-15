using UnityEngine;
using System.Collections;

public abstract class StatusEffect : MonoBehaviour 
{
    public abstract void inflictStatus(Damageable dmg);
}
