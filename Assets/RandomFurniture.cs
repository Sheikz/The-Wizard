using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RandomFurniture : MonoBehaviour 
{
	void Start() 
	{
        //instantiateRandomFurniture();
	}

    public void instantiate()
    {
        List<ItemWithDropChance> validFurnitures = new List<ItemWithDropChance>();

        foreach (ItemWithDropChance furnitureToTest in WorldManager.instance.furnitures)
        {
            if (spaceExistsFor(furnitureToTest.item, GameManager.instance.layerManager.blockingLayer))   // Check if there is enough space to put the furniture
                validFurnitures.Add(furnitureToTest);
        }

        GameObject toInstantiate = ItemWithDropChance.getItem(validFurnitures).item;
        if (toInstantiate == null)
            return;

        GameObject newFurniture = Instantiate(toInstantiate, transform.position, Quaternion.identity) as GameObject;
        newFurniture.transform.parent = transform.parent;
    }

    private bool spaceExistsFor(GameObject item, LayerMask layerMask)
    {
        if (item == null)
            return true;
        BoxCollider2D[] boxColliders = item.GetComponentsInChildren<BoxCollider2D>();
        CircleCollider2D[] circleColliders = item.GetComponentsInChildren<CircleCollider2D>();
        foreach (BoxCollider2D col in boxColliders)
        {
            Vector2 boxCenter = (Vector2)(transform.position + col.transform.localPosition) + col.offset;
            if (Physics2D.OverlapBoxNonAlloc(boxCenter, col.size, 0, null, layerMask) > 0)  // Check if one of the colliders intersect with something
                return false;
        }
        foreach (CircleCollider2D col in circleColliders)
        {
            Vector2 circleCenter = (Vector2)(transform.position + col.transform.localPosition) + col.offset;
            if (Physics2D.OverlapCircleNonAlloc(circleCenter, col.radius, null, layerMask) > 0)
                return false;
        }
        return true;
    }
}
