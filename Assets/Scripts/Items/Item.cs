using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Collider2D))]
public abstract class Item : MonoBehaviour
{
    private bool canBePickedUp = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canBePickedUp)
            isPickedUpBy(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        canBePickedUp = true;
    }

    protected abstract void isPickedUpBy(Collider2D other);

    /// <summary>
    /// Avoid infinite picking if an item is dropped at the time it's picked up
    /// </summary>
    protected void deactivatePickupUntilLeftCollider()
    {
        canBePickedUp = false;
    }
}