using UnityEngine;
using System.Collections;

public class SpellLevelUpButton : MonoBehaviour
{
    public SpellBookSpell spell;

	void Start ()
    {
        spell = GetComponentInParent<SpellBookSpell>();
	}
	
    void OnClick()
    {
        spell.levelUp();
    }
}
