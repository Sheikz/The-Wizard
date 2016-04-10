using UnityEngine;
using System.Collections.Generic;
using System;

public class SlowTime : MonoBehaviour
{
    public float timeMultiplier = 0.3f;
    private List<MovingCharacter> affectedCharacters;
    private List<MovingSpell> affectedProjectiles;

    void Awake()
    {
        affectedProjectiles = new List<MovingSpell>();
        affectedCharacters = new List<MovingCharacter>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        MovingSpell spell = collision.GetComponent<MovingSpell>();
        if (spell && !affectedProjectiles.Contains(spell))
            addProjectile(spell);

        MovingCharacter character = collision.GetComponent<MovingCharacter>();
        if (character && !affectedCharacters.Contains(character))
            addCharacter(character);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        MovingSpell spell = collision.GetComponent<MovingSpell>();
        if (spell && affectedProjectiles.Contains(spell))
            removeProjectile(spell);

        MovingCharacter character = collision.GetComponent<MovingCharacter>();
        if (character && affectedCharacters.Contains(character))
            removeCharacter(character);
    }

    private void addProjectile(MovingSpell spell)
    {
        spell.speed = spell.speed * timeMultiplier;
        spell.refreshSpeed();
        affectedProjectiles.Add(spell);
    }

    private void addCharacter(MovingCharacter character)
    {
        character.speed = character.speed * timeMultiplier;
        character.anim.speed = character.anim.speed * timeMultiplier;
        affectedCharacters.Add(character);
    }

    private void removeCharacter(MovingCharacter character)
    {
        character.speed = character.speed / timeMultiplier;
        character.anim.speed = character.anim.speed / timeMultiplier;
        affectedCharacters.Remove(character);
    }

    private void removeProjectile(MovingSpell spell)
    {
        spell.speed = spell.speed / timeMultiplier;
        spell.refreshSpeed();
        affectedProjectiles.Remove(spell);
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        foreach (MovingSpell spell in affectedProjectiles)
        {
            spell.speed = spell.speed / timeMultiplier;
            spell.refreshSpeed();
        }

        foreach (MovingCharacter character in affectedCharacters)
        {
            character.speed = character.speed / timeMultiplier;
            if (character.anim)
                character.anim.speed = character.anim.speed / timeMultiplier;
        }
    }
}
