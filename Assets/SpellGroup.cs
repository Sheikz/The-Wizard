using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellGroup : MonoBehaviour 
{
    public Transform[] spells;

    void Awake()
    {
        spells = new Transform[7];
        spells[(int)MagicElement.Light] = transform.FindChild("Light");
        spells[(int)MagicElement.Air] = transform.FindChild("Air");
        spells[(int)MagicElement.Fire] = transform.FindChild("Fire");
        spells[(int)MagicElement.Arcane] = transform.FindChild("Arcane");
        spells[(int)MagicElement.Ice] = transform.FindChild("Ice");
        spells[(int)MagicElement.Earth] = transform.FindChild("Earth");
        spells[(int)MagicElement.Shadow] = transform.FindChild("Shadow");
    }
}
