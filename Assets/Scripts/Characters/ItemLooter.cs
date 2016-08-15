using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class ItemLooter : MonoBehaviour
{
    public float lootDistance = 2f;
    [HideInInspector]
    public Inventory inventory;

    private PlayerController player;

    void Awake()
    {
        inventory = GetComponentInParent<Inventory>();
        if (!inventory)
            Debug.LogWarning("Inventory not found in parent");

        player = GetComponentInParent<PlayerController>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (!item || !item.canBePickedUp)
            return;

        if (item.autoPickup)
            item.pullToSelf(this);
    }

    public void pickup(Item item)
    {
        if ((transform.position - item.transform.position).sqrMagnitude <= lootDistance * lootDistance)
        {
            item.pullToSelf(this);
        }
        else if (player)  // Too far away. Need to move closer
        {
            StartCoroutine(player.moveToItemAndPickup(item, this));
        }
    }
}
