using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a hole that kill a monster if it falls in
/// </summary>
public class Hole : MonoBehaviour
{
    public float damageRatio = 0.2f; // Damage % falling into this hole inflicts
    private Collider2D[] colliders;

    private List<MovingCharacter> charactersInside;

    void Start()
    {
        colliders = GetComponents<Collider2D>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        MovingCharacter character = collision.GetComponent<MovingCharacter>();
        if (!character || character.isFlying)
            return;

        foreach (Collider2D col in colliders)
        {
            if (col.bounds.Contains(character.transform.position))
            {
                character.startFalling(damageRatio);
                return;
            }
        }
    }
}
