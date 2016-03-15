using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovingCharacter), typeof(Rigidbody2D))]
public class Knockbackable : MonoBehaviour
{
    public float knockbackImmuneTime;

    private bool isImmuneKnockback;
    private Rigidbody2D rb;
    private MovingCharacter character;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        character = GetComponent<MovingCharacter>();
    }

    void Start ()
    {
        isImmuneKnockback = false;
    }

    public void knockback(Vector2 force, float duration)
    {
        if (!isImmuneKnockback)
        {
            character.stopMovementFor(duration);
            StartCoroutine(immunizeKnockback(knockbackImmuneTime));
            rb.AddForce(force);
        }
    }

    private IEnumerator immunizeKnockback(float time)
    {
        isImmuneKnockback = true;
        yield return new WaitForSeconds(time);
        isImmuneKnockback = false;
    }

}
