using UnityEngine;
using System.Collections.Generic;
using System;

public class TypeGroup : MonoBehaviour
{
    public Transform[,] spells;

    void Awake()
    {
        spells = new Transform[7, 2];
        spells[(int)MagicElement.Light, (int)SpellSet.SpellSet1] = transform.FindChild("Light/SpellSet1");
        spells[(int)MagicElement.Light, (int)SpellSet.SpellSet2] = transform.FindChild("Light/SpellSet2");
        spells[(int)MagicElement.Air, (int)SpellSet.SpellSet1] = transform.FindChild("Air/SpellSet1");
        spells[(int)MagicElement.Air, (int)SpellSet.SpellSet2] = transform.FindChild("Air/SpellSet2");
        spells[(int)MagicElement.Fire, (int)SpellSet.SpellSet1] = transform.FindChild("Fire/SpellSet1");
        spells[(int)MagicElement.Fire, (int)SpellSet.SpellSet2] = transform.FindChild("Fire/SpellSet2");
        spells[(int)MagicElement.Arcane, (int)SpellSet.SpellSet1] = transform.FindChild("Arcane/SpellSet1");
        spells[(int)MagicElement.Arcane, (int)SpellSet.SpellSet2] = transform.FindChild("Arcane/SpellSet2");
        spells[(int)MagicElement.Ice, (int)SpellSet.SpellSet1] = transform.FindChild("Ice/SpellSet1");
        spells[(int)MagicElement.Ice, (int)SpellSet.SpellSet2] = transform.FindChild("Ice/SpellSet2");
        spells[(int)MagicElement.Earth, (int)SpellSet.SpellSet1] = transform.FindChild("Earth/SpellSet1");
        spells[(int)MagicElement.Earth, (int)SpellSet.SpellSet2] = transform.FindChild("Earth/SpellSet2");
        spells[(int)MagicElement.Shadow, (int)SpellSet.SpellSet1] = transform.FindChild("Shadow/SpellSet1");
        spells[(int)MagicElement.Shadow, (int)SpellSet.SpellSet2] = transform.FindChild("Shadow/SpellSet2");
    }
}
