using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class ItemLooter : MonoBehaviour
{
    public float sqrDistanceLoot = 0.5f * 0.5f;
    [HideInInspector]
    public Inventory inventory;

    void Awake()
    {
        inventory = GetComponentInParent<Inventory>();
        if (!inventory)
            Debug.LogWarning("Inventory not found in parent");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (!item || !item.canBePickedUp)
            return;

        item.pullToSelf(this);
    }
}
