using UnityEngine;
using System.Collections.Generic;

public class TypeGroup : MonoBehaviour
{
    public Transform[] spells;

    void Awake()
    {
        spells = new Transform[7];
        spells[(int)MagicElement.Light] =  transform.FindDeepChild("LightSpells");
        spells[(int)MagicElement.Air] = transform.FindDeepChild("AirSpells");
        spells[(int)MagicElement.Fire] = transform.FindDeepChild("FireSpells");
        spells[(int)MagicElement.Arcane] = transform.FindDeepChild("ArcaneSpells");
        spells[(int)MagicElement.Ice] = transform.FindDeepChild("IceSpells");
        spells[(int)MagicElement.Earth] = transform.FindDeepChild("EarthSpells");
        spells[(int)MagicElement.Shadow] = transform.FindDeepChild("ShadowSpells");
    }
}
