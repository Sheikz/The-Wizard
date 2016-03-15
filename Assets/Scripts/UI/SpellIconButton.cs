using UnityEngine;
using System.Collections;

public class SpellIconButton : MonoBehaviour
{
    private SpellBookSpell spBookSpell;
	// Use this for initialization
	void Start ()
    {
        spBookSpell = GetComponentInParent<SpellBookSpell>();
	}

    public void onClick()
    {
        spBookSpell.onClick();
    }
}
