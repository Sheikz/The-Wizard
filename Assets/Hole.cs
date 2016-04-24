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
    /*
    void Update()
    {
        for (int i = charactersInside.Count - 1; i >= 0; i --)
        {
            if (!charactersInside[i])
            {
                charactersInside.RemoveAt(i);
                continue;
            }
            foreach (Collider2D col in colliders)
            {
                if (col.bounds.Contains(charactersInside[i].transform.position))
                {
                    if (!charactersInside[i].isFlying)
                        charactersInside[i].startFalling(damageRatio);
                    break;
                }
            }
        }
    }*/
    /*
    void OnTriggerEnter2D(Collider2D collision)
    {
        MovingCharacter character = collision.gameObject.GetComponent<MovingCharacter>();
        if (!charactersInside.Contains(character))
            charactersInside.Add(character);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        MovingCharacter character = collision.gameObject.GetComponent<MovingCharacter>();
        charactersInside.Remove(character);
    }*/

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
