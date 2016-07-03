using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextOnMouseOver : MonoBehaviour 
{
    public GameObject floatingTextPrefab;

    private FloatingText floatingText;
    private EquipableItem equipableItem;

    void Awake()
    {
        equipableItem = GetComponentInParent<EquipableItem>();
    }

    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    void OnMouseEnter()
    {
        hoveringOverItem();
    }

    void hoveringOverItem()
    {
        InputManager.instance.isMouseHoveringItem = true;
        InputManager.instance.hoveringOverItem = equipableItem;
        if (!equipableItem)
            return;

        if (floatingText)
            floatingText.show(true);
        else
        {
            floatingText = Instantiate(floatingTextPrefab).GetComponentInChildren<FloatingText>();
            floatingText.initialize(gameObject, equipableItem.itemStats.name);
            floatingText.speed = 0;
            floatingText.setColor(ItemManager.rarityColors[(int)equipableItem.itemStats.rarity]);
        }
    }

    void OnMouseExit()
    {
        InputManager.instance.isMouseHoveringItem = false;
        InputManager.instance.hoveringOverItem = null;

        if (floatingText)
            floatingText.show(false);
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        if (floatingText)
            Destroy(floatingText.gameObject);
    }
}
